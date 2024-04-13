using Discord;
using Discord.WebSocket;
using DiscordBot.Model.Storage;

namespace DiscordBot.Model;

public record PFNotificationRequest(
    DataCenter DataCenter,
    string DutyName,
    List<List<Job>> PartyComposition,
    DateTime CreationTime,
    ISocketMessageChannel Channel,
    SocketUser User
    ) : Entity
{

}