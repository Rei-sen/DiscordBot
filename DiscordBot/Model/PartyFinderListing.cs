using DiscordBot.Model.Storage;

namespace DiscordBot.Model;

public class PartyFinderListing : Entity
{
    public string CreatorName { get; set; }
    public string DataCenter { get; set; }
    public string Category { get; set; }
    public string DutyName { get; set; }
    public string Description { get; set; }
    public string Tag { get; set; }
    public List<Slot> PartySlots { get; set; }
    public uint MinItemLevel { get; set; }
    public TimeSpan TimeUntilExpiration { get; set; }
    public TimeSpan TimeSinceLastUpdate { get; set; }
}