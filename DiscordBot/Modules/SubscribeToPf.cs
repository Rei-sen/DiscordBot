using Discord;
using Discord.Interactions;
using DiscordBot.Model;
using DiscordBot.Model.DB;
using DiscordBot.Model.Storage;

namespace DiscordBot.Modules;

public class SubscribeToPF(
    IInMemoryRepository<PFListing> _pfRepository,
    IInMemoryRepository<PFNotificationRequest> _notificationRepository,
    //IRepository<PFSubscription> _subscriptionRepository,
    PFSubscriptionsContext _subscriptionContext
    ) : InteractionModuleBase<SocketInteractionContext>
{
    Dictionary<string, string> _knownDutyShorthand = new()
    {
        {"ucob", "The Unending Coil of Bahamut (Ultimate)" },
        {"uwu", "The Weapon's Refrain (Ultimate)" },
        {"tea", "The Epic of Alexander (Ultimate)" },
        {"dsr", "Dragonsong's Reprise (Ultimate)" },
        {"top", "The Omega Protocol (Ultimate)" }
    };

    [SlashCommand("subscribetopf", "Subscribe this channel to PF listings")]
    public async Task SubscribeChannelToListings(DataCenter dc, string dutyName, string color)
    {

        if (_knownDutyShorthand.TryGetValue(dutyName.ToLower(), out var dutyFullName))
        {
            dutyName = dutyFullName;
        }

        var message = await Context.Channel.SendMessageAsync(embed: new EmbedBuilder().AddField("...", "...").Build());
        await RespondAsync("Subscribed to PF listings! The listings will get updated soon!", ephemeral: true);

        PFSubscription subscription = new PFSubscription(Context.Channel.Id, message.Id, dc, dutyName, color);

        _subscriptionContext.Add(subscription);
        _subscriptionContext.SaveChanges();

        var listings = subscription.GetSubscriptionEmbedBuilder().Build();
        await message.ModifyAsync(message => message.Embed = listings);
    }

    [SlashCommand("pfalert", "Get notified when a PF listing matching the specified criteria is posted")]
    public async Task SubscribeToPFAlert(DataCenter dc, string dutyName, string partyComposition)
    {
        if (_knownDutyShorthand.TryGetValue(dutyName.ToLower(), out var dutyFullName))
        {
            dutyName = dutyFullName;
        }

        await RespondAsync("You'll get notified when a party matching your criteria is created.", ephemeral: true);

        partyComposition = partyComposition.Trim().ToLower();

        var party = partyComposition.Split(',').Select(player => ParsePlayerJobPreferences(player)).ToList();
        var request = new PFNotificationRequest(dc, dutyName, party, DateTime.Now, Context.Channel, Context.User);

        await _notificationRepository.Insert(request);
    }

    private readonly Dictionary<string, List<Job>> _knownShorthands = new()
        {
            { "tank", new List<Job> { Job.PLD, Job.WAR, Job.DRK, Job.GNB} },
            { "healer", new List<Job> { Job.WHM, Job.SCH, Job.AST, Job.SGE } },
            { "dps", new List<Job> { Job.MNK, Job.DRG, Job.NIN, Job.SAM, Job.RPR, Job.BRD, Job.MCH, Job.DNC, Job.SMN, Job.RDM, Job.BLM } },
            { "melee", new List<Job> { Job.MNK, Job.DRG, Job.NIN, Job.SAM, Job.RPR } },
            { "physranged", new List<Job> { Job.BRD, Job.MCH, Job.DNC } },
            { "caster", new List<Job> { Job.SMN, Job.RDM, Job.BLM } },
        };

    private List<Job> ParsePlayerJobPreferences(string requestedJobs)
    {
        var jobList = requestedJobs.Split('|').Select(job => job.Trim());

        List<Job> result = new List<Job>();
        foreach (var job in jobList)
        {
            if (_knownShorthands.TryGetValue(job, out var shorthandJobs))
            {
                result.AddRange(shorthandJobs);
            }
            else
            {
                if (Enum.TryParse<Job>(job, true, out var parsedJob))
                {
                    result.Add(parsedJob);
                }
            }
        }

        return result;
    }
}
