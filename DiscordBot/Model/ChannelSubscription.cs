using DiscordBot.Model.Storage;

internal record ChannelSubscription(ulong ChannelId, ulong MessageId, string DataCenter) : Entity;
