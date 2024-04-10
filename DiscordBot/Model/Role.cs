namespace DiscordBot.Model;

[Flags]
public enum Role
{
    Tank = 1,
    Healer = 2,
    DPS = 4,
    Other = 8,
}