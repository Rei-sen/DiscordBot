using Discord;
using Discord.Interactions;
using DiscordBot.Model;
using DiscordBot.Model.Storage;

namespace DiscordBot.Modules;

public class SubscribeToPf(IInMemoryRepository<PartyFinderListing> repository) : InteractionModuleBase<SocketInteractionContext>
{
   
    [SlashCommand("subscribetopf", "Subscribe this channel to PF listings")]
    public async Task SubscribeChannelToListings()
    {
        var listings = await GetEmbedAsync();

        await RespondAsync(embed: listings, ephemeral: true);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Generates an embed containing the party finder listings.
    /// </summary>
    /// <returns>An Embed object representing the party finder listings.</returns>
    public async Task<Embed> GetEmbedAsync()
    {
        var listings = await repository.Query();
        Console.WriteLine("Generating party finder embed...");
        Console.WriteLine(listings.Count());
        
        var builder = new EmbedBuilder()
            .WithTitle("PF Listings")
            .WithColor(Color.Blue);
        int maxListings = EmbedBuilder.MaxFieldCount / 3 - 1;
        foreach (var l in listings.Take(maxListings))
        {
            builder.AddField(l.CreatorName, string.Join(" ", l.PartySlots.Select(s => s.GetEmoji())), true)
                .AddField("tag here", l.Description.Equals("") ? "\u200b" : l.Description, true)
                .AddField($"{l.TimeSinceLastUpdate} ago", $"Expires in {l.TimeUntilExpiration}", true);
        }

        if (listings.Count() - maxListings > 0)
        {
            builder.WithFooter($"{listings.Count() - maxListings} not shown!");
        }

        return builder.Build();
    }
}
