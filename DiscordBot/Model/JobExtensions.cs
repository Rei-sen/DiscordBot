using DiscordBot.Services;

namespace DiscordBot.Model;

public static class JobExtensions
{
    public static Job JobStringToJob(string s) =>
       Enum.TryParse(s, out Job job) ? job : Job.Unknown;


    private static readonly Dictionary<Job, string> _emojiDict = new Dictionary<Job, string>()
    {
        { Job.PLD, "<:paladin:1227407318652686387>" },
        { Job.WAR, "<:warrior:1227407518637101096>" },
        { Job.DRK, "<:darkknight:1227407134438985788>" },
        { Job.GNB, "<:gunbreaker:1227407182472155146>" },
        { Job.WHM, "<:whitemage:1227407519840866324>" },
        { Job.SCH, "<:scholar:1227407398314971136>" },
        { Job.AST, "<:astrologian:1227407075542569020>" },
        { Job.SGE, "<:sage:1227407396188590110>" },
        { Job.MNK, "<:monk:1227407275459612732>" },
        { Job.DRG, "<:dragoon:1227407136674414653>" },
        { Job.NIN, "<:ninja:1227407317813821560>" },
        { Job.SAM, "<:samurai:1227407397195223130>" },
        { Job.RPR, "<:reaper:1227407362675970089>" },
        { Job.BRD, "<:bard:1227407076561784904>" },
        { Job.MCH, "<:machinist:1227407273031110687>" },
        { Job.DNC, "<:dancer:1227407133004533782>" },
        { Job.SMN, "<:summoner:1227407435526832128>" },
        { Job.RDM, "<:redmage:1227407363770683433>" },
        { Job.BLM, "<:blackmage:1227407077710888990>" },
        { Job.BLU, "<:bluemage:1227407078373593129>" },
        { Job.GLD, "<:gladiator:1227407181268389898>" },
        { Job.MRD, "<:marauder:1227407274356637696>" },
        { Job.CNJ, "<:conjurer:1227407104264896512>" },
        { Job.PGL, "<:pugilist:1227407320300916797>" },
        { Job.LNC, "<:lancer:1227407272213221428>" },
        { Job.ROG, "<:rogue:1227407365008003082>" },
        { Job.ARC, "<:archer:1227407074552451152>" },
        { Job.ACN, "<:arcanist:1227407030214459394>" },
        { Job.THM, "<:thaumaturge:1227407466405564447>" },
    };
    public static string GetEmoji(Job job)
    {
        return _emojiDict.TryGetValue(job, out string? emoji) ? emoji : "<:nonetaken:1227407859285753996>";
    }
}