using DiscordBot.Model.Storage;

namespace DiscordBot.Model;

internal class ChannelSubscription(ulong channelId, ulong messageId, string dataCenter) : Entity
{
    public ulong ChannelId { get; set; } = channelId;
    public ulong MessageId { get; set; } = messageId;
    public string DataCenter { get; set; } = dataCenter;
}