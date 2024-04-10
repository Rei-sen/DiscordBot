using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using System.Net;
using DiscordBot.Model;
using DiscordBot.Model.Storage;

namespace DiscordBot.Services;

internal class PartyFinderService(IInMemoryRepository<PartyFinderListing> repository) : IHostedService
{
    // todo: HttpClient should be singleton because you can use up all the sockets in the Operation System
    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://xivpf.com"),
    };

    private CancellationTokenSource? _cancellationTokenSource;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource = new();
        Task.Run(async () =>
        {
            await UpdateLatestPartyFinderListings();
            if (_cancellationTokenSource.Token.IsCancellationRequested)
                return;
            
            var timer = new PeriodicTimer(TimeSpan.FromSeconds(30));
            while (await timer.WaitForNextTickAsync(_cancellationTokenSource.Token))
            {
                if (_cancellationTokenSource.Token.IsCancellationRequested)
                    return;
                await UpdateLatestPartyFinderListings();
            }
        }, _cancellationTokenSource.Token);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _cancellationTokenSource?.Cancel();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates the latest party finder listings by making a request to the external website and parsing the HTML content.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateLatestPartyFinderListings()
    {
        Console.WriteLine("Updating party finder listings...");
        HttpResponseMessage response = await _httpClient.GetAsync("listings");
        string htmlContent = await response.Content.ReadAsStringAsync();

        await repository.Clear();
        foreach (var listing in GetListings(htmlContent))
        {
            await repository.Insert(listing);
        }
    }

    /// <summary>
    /// Parses the HTML content and extracts the party finder listings.
    /// </summary>
    /// <param name="htmlContent">The HTML content to parse.</param>
    /// <returns>A list of PartyFinderListing objects representing the extracted party finder listings.</returns>
    private IEnumerable<PartyFinderListing> GetListings(string htmlContent)
    {
        List<PartyFinderListing> listings = new List<PartyFinderListing>();

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        var listingNodes = doc.DocumentNode.SelectNodes("//div[@class='listing']");
        if (listingNodes == null)
            yield break;

        foreach (HtmlNode listingNode in listingNodes)
        {
            PartyFinderListing listing = new PartyFinderListing();

            // Parse creator name
            listing.CreatorName = listingNode.SelectSingleNode(".//div[@class='item creator']/span[@class='text']")?.InnerText ?? "Unknown";

            // Parse data center
            listing.DataCenter = listingNode.GetAttributeValue("data-centre", "");

            // Parse category
            listing.Category = listingNode.GetAttributeValue("data-pf-category", "");

            // Parse duty name
            listing.DutyName = WebUtility.HtmlDecode(listingNode.SelectSingleNode("./div[@class='left']/div[@class='duty cross']")?.InnerText) ?? "";

            // Parse description and tag
            listing.Description = WebUtility.HtmlDecode(listingNode.SelectSingleNode("./div[@class='left']/div[@class='description']")?.InnerText.Trim()) ?? "";

            // Parse party slots
            var slotNodes = listingNode.SelectSingleNode("./div[@class='left']/div[@class='party']");
            listing.PartySlots = ParsePartyNode(slotNodes);

            // Parse minimum item level
            string minItemLevelStr = listingNode.SelectSingleNode(".//div[@class='middle']//div[@class='value']")?.InnerText ?? "0";
            uint.TryParse(minItemLevelStr, out uint minItemLevel);
            listing.MinItemLevel = minItemLevel;

            // Parse time until expiration
            string expiresStr = listingNode.SelectSingleNode(".//div[@class='item expires']/span[@class='text']")?.InnerText ?? "0";
            listing.TimeUntilExpiration = ParseTimeSpan(expiresStr);

            // Parse time since last update
            string updatedStr = listingNode.SelectSingleNode(".//div[@class='item updated']/span[@class='text']")?.InnerText ?? "0";
            listing.TimeSinceLastUpdate = ParseTimeSpan(updatedStr);

            yield return listing;
        }
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
            var slot = new Slot(availableJobs, isFree);

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
