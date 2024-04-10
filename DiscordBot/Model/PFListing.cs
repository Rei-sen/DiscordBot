using DiscordBot.Model.Storage;

namespace DiscordBot.Model;

public record PFListing(
    string CreatorName,
    DataCenter DataCenter,
    string Category,
    string DutyName,
    string Description,
    string? Tag,
    List<Slot> PartySlots,
    uint MinItemLevel,
    TimeSpan TimeUntilExpiration,
    TimeSpan TimeSinceLastUpdate
    ) : Entity
{

}
