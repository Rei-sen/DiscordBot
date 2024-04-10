using Discord;
using DiscordBot.Model.Storage;
using System.ComponentModel.DataAnnotations;

namespace DiscordBot.Model;

public class PFSubscription
{
    [Key]
    public int Id { get; set; }
    public ulong ChannelId { get; init; }
    public ulong MessageId { get; init; }
    public DataCenter DataCenter { get; init; }
    public string Duty { get; init; }
    public string Color { get; init; }

    public PFSubscription(ulong channelId, ulong messageId, DataCenter dataCenter, string duty, string color)
    {
        ChannelId = channelId;
        MessageId = messageId;
        DataCenter = dataCenter;
        Duty = duty;
        Color = color;
    }

    public EmbedBuilder GetSubscriptionEmbedBuilder()
    {
        Color listingColor = (Color)System.Drawing.Color.FromName(Color);
        var builder = new EmbedBuilder()
            .WithTitle($"{Duty} - {DataCenter}")
            .WithColor(listingColor);

        return builder;
    }
}
