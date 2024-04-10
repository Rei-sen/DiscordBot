using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using System.Net;
using DiscordBot.Model;
using DiscordBot.Model.Storage;
using DiscordBot.Modules;
using Discord;
using Discord.WebSocket;

namespace DiscordBot.Services;

/// <summary>
/// Represents a service for updating and managing party finder listings.
/// </summary>
internal class PartyFinderService(
    DiscordSocketClient _client,
    IInMemoryRepository<PFListing> _pfListingRepository,
    IRepository<PFSubscription> _pfSubscriptionRepository
    ) : IHostedService
{
    // todo: HttpClient should be singleton because you can use up all the sockets in the Operation System
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://xivpf.com"),
    };

    private CancellationTokenSource? _cancellationTokenSource;

    /// <summary>
    /// Starts the party finder service asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = new();
        Task.Run(async () =>
        {
            await UpdatePartyFinderListings();
            if (_cancellationTokenSource.Token.IsCancellationRequested)
                return;

            var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
            while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;
                await UpdatePartyFinderListings();
            }
        }, _cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the party finder service asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource?.Cancel();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates the party finder listings and sends updated listings to subscribed channels.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdatePartyFinderListings()
    {
        await _pfListingRepository.Clear();
        await UpdateLatestPartyFinderState();

        foreach (var subscription in await _pfSubscriptionRepository.Query())
        {
            var listings = await GetEmbedAsync(subscription);
            var channel = await _client.GetChannelAsync(subscription.ChannelId) as IMessageChannel;
            if (channel is null)
            {
                await _pfSubscriptionRepository.Delete(subscription);
                continue;
            }

            var message = await channel.GetMessageAsync(subscription.MessageId) as IUserMessage;
            if (message is null)
            {
                await _pfSubscriptionRepository.Delete(subscription);
                continue;
            }

            await message.ModifyAsync(message => message.Embed = listings);
        }
    }

    /// <summary>
    /// Gets the embed containing party finder listings for a specific subscription.
    /// </summary>
    /// <param name="subscription">The party finder subscription.</param>
    /// <returns>The embed containing the party finder listings.</returns>
    public async Task<Embed> GetEmbedAsync(PFSubscription subscription)
    {
        var listings = await _pfListingRepository.Query();

        var builder = subscription.getSubscriptionEmbedBuilder();

        var allMatchingListings = listings
            .Where(l => l.DataCenter == subscription.DataCenter)
            .Where(l => l.DutyName.Contains(subscription.Duty))
            .ToList();

        int maxListings = EmbedBuilder.MaxFieldCount / 3;

        foreach (var l in allMatchingListings.Take(maxListings))
        {
            var expirationTimeStamp = TimestampTag.FormatFromDateTime(DateTime.Now + l.TimeUntilExpiration, TimestampTagStyles.Relative);
            var lastUpdateTimeStamp = TimestampTag.FormatFromDateTime(DateTime.Now - l.TimeSinceLastUpdate, TimestampTagStyles.Relative);
            builder.AddField(l.CreatorName, string.Join(" ", l.PartySlots.Select(s => s.GetEmoji())), true)
                .AddField(l.Tag is null ? "\u200b" : l.Tag, l.Description.Equals("") ? "\u200b" : l.Description, true)
                .AddField($"<:stopwatch:1227407434390437938>{lastUpdateTimeStamp}", $"<:hourglass:1227407231050584125>{expirationTimeStamp}", true);
        }

        var remainingListingCount = allMatchingListings.Count() - maxListings;

        if (remainingListingCount > 0)
        {
            builder.WithFooter($"+ {remainingListingCount} more listings not shown!");
        }

        return builder.Build();
    }

    /// <summary>
    /// Updates the latest party finder listings by making a request to the external website and parsing the HTML content.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateLatestPartyFinderState()
    {
        HttpResponseMessage response = await _httpClient.GetAsync("listings");
        string htmlContent = await response.Content.ReadAsStringAsync();

        await _pfListingRepository.Clear();
        foreach (var listing in GetListings(htmlContent))
        {
            await _pfListingRepository.Insert(listing);
        }
    }

    /// <summary>
    /// Parses the HTML content and extracts the party finder listings.
    /// </summary>
    /// <param name="htmlContent">The HTML content to parse.</param>
    /// <returns>A list of PartyFinderListing objects representing the extracted party finder listings.</returns>
    private IEnumerable<PFListing> GetListings(string htmlContent)
    {
        List<PFListing> listings = new List<PFListing>();

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        var listingNodes = doc.DocumentNode.SelectNodes("//div[@class='listing']");
        if (listingNodes == null)
            yield break;

        foreach (HtmlNode listingNode in listingNodes)
        {
            PFListing listing = ParseListingNode(listingNode);
            yield return listing;
        }
    }

    /// <summary>
    /// Parses a single listing node and extracts the party finder listing.
    /// </summary>
    /// <param name="listingNode">The HTML node representing a party finder listing.</param>
    /// <returns>A PartyFinderListing object representing the extracted party finder listing.</returns>
    private PFListing ParseListingNode(HtmlNode listingNode)
    {
        var creatorName = listingNode.SelectSingleNode(".//div[@class='item creator']/span[@class='text']")?.InnerText ?? "Unknown";
        var dataCenterStr = listingNode.GetAttributeValue("data-centre", "");
        var dataCenter = Enum.TryParse<DataCenter>(dataCenterStr, out DataCenter parsedDataCenter) ? parsedDataCenter : DataCenter.Unknown;
        var category = listingNode.GetAttributeValue("data-pf-category", "");
        var dutyName = WebUtility.HtmlDecode(listingNode.SelectSingleNode("./div[@class='left']/div[@class='duty cross']")?.InnerText) ?? "";
        var tag = WebUtility.HtmlDecode(listingNode.SelectSingleNode("./div[@class='left']/div[@class='description']/span")?.InnerText.Trim());
        var description = WebUtility.HtmlDecode(listingNode.SelectSingleNode("./div[@class='left']/div[@class='description']")?.InnerText.Trim()) ?? "";
        if (tag is not null)
        {
            description = description.Replace(tag, "").Trim();
        }
        var slotNodes = listingNode.SelectSingleNode("./div[@class='left']/div[@class='party']");
        var partySlots = ParsePartyNode(slotNodes);
        var minItemLevelStr = listingNode.SelectSingleNode(".//div[@class='middle']//div[@class='value']")?.InnerText ?? "0";
        uint.TryParse(minItemLevelStr, out uint minItemLevel);
        var expiresStr = listingNode.SelectSingleNode(".//div[@class='item expires']/span[@class='text']")?.InnerText ?? "0";
        var timeUntilExpiration = ParseTimeSpan(expiresStr);
        var updatedStr = listingNode.SelectSingleNode(".//div[@class='item updated']/span[@class='text']")?.InnerText ?? "0";
        var timeSinceLastUpdate = ParseTimeSpan(updatedStr);

        return new PFListing(creatorName, dataCenter, category, dutyName, description, tag, partySlots, minItemLevel, timeUntilExpiration, timeSinceLastUpdate);
    }

    /// <summary>
    /// Parses the HTML content and extracts the party composition.
    /// </summary>
    /// <param name="partyNode">The HTML node containing the party information.</param>
    /// <returns>A list of Slot objects representing the extracted party slots.</returns>
    private List<Slot> ParsePartyNode(HtmlNode partyNode)
    {
        List<Slot> partySlots = new List<Slot>();

        foreach (HtmlNode slotNode in partyNode.SelectNodes("./div[not(@class='total')]"))
        {
            var isFree = slotNode.GetAttributeValue("class", "").Contains("filled") ? false : true;
            var availableJobs = slotNode.GetAttributeValue("title", "").Split(' ').ToList();
            var jobList = availableJobs.Select(j => JobExtensions.JobStringToJob(j)).ToList();
            var slot = new Slot(jobList, isFree);

            partySlots.Add(slot);
        }

        return partySlots;
    }

    /// <summary>
    /// Parses the time string and converts it to a TimeSpan object.
    /// </summary>
    /// <param name="timeStr">The time string to parse.</param>
    /// <returns>A TimeSpan object representing the parsed time.</returns>
    // todo: There's a library for this (moment.net)
    private TimeSpan ParseTimeSpan(string timeStr)
    {

        if (timeStr.Equals("in an hour"))
        {
            return TimeSpan.FromHours(1);
        }
        else if (timeStr.Equals("in a minute"))
        {
            return TimeSpan.FromMinutes(1);
        }
        else if (timeStr.StartsWith("in"))
        {
            // Time until expiration format: "in X minutes"
            var split = timeStr.Split(' ');
            int time = int.Parse(split[1]);

            if (split[2].Equals("minutes"))
            {
                return TimeSpan.FromMinutes(time);
            }
            else
            {
                return TimeSpan.FromSeconds(time);
            }
        }
        else if (timeStr.Equals("a minute ago"))
        {
            return TimeSpan.FromMinutes(1);
        }
        else if (timeStr.Equals("an hour ago"))
        {
            return TimeSpan.FromHours(1);
        }
        else if (timeStr.Equals("now"))
        {
            return TimeSpan.Zero;
        }
        else
        {
            // Time since last update format: "X minutes ago"
            var split = timeStr.Split(' ');
            int time = int.Parse(split[0]);
            string unit = split[1];

            if (unit.EndsWith("s"))
            {
                unit = unit[..^1]; // Remove the 's' at the end
            }

            switch (unit)
            {
                case "minute":
                    return TimeSpan.FromMinutes(time);
                case "hour":
                    return TimeSpan.FromHours(time);
                case "second":
                    return TimeSpan.FromSeconds(time);
                default:
                    throw new ArgumentException("Invalid time unit.");
            }
        }
    }
}
