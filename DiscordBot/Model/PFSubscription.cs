using Discord;
using DiscordBot.Model.Storage;

namespace DiscordBot.Model;

public record PFSubscription(
    ulong ChannelId,
    ulong MessageId,
    DataCenter DataCenter,
    string Duty,
    string Color
    ) : Entity
{

    public EmbedBuilder getSubscriptionEmbedBuilder()
    {
        Color listingColor = (Color)System.Drawing.Color.FromName(Color);
        var builder = new EmbedBuilder()
            .WithTitle($"{Duty} - {DataCenter}")
            .WithColor(listingColor);

        return builder;
    }
}