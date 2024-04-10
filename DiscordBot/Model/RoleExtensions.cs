using DiscordBot.Services;

namespace DiscordBot.Model;

public static class RoleExtensions
{
    public static Role FromJob(Job jobRole)
    {
        return jobRole switch
        {
            Job.PLD => Role.Tank,
            Job.WAR => Role.Tank,
            Job.DRK => Role.Tank,
            Job.GNB => Role.Tank,

            Job.GLD => Role.Tank,
            Job.MRD => Role.Tank,

            Job.WHM => Role.Healer,
            Job.SCH => Role.Healer,
            Job.AST => Role.Healer,
            Job.SGE => Role.Healer,

            Job.CNJ => Role.Healer,

            Job.MNK => Role.DPS,
            Job.DRG => Role.DPS,
            Job.NIN => Role.DPS,
            Job.SAM => Role.DPS,
            Job.RPR => Role.DPS,

            Job.PGL => Role.DPS,
            Job.LNC => Role.DPS,
            Job.ROG => Role.DPS,

            Job.BRD => Role.DPS,
            Job.MCH => Role.DPS,
            Job.DNC => Role.DPS,

            Job.ARC => Role.DPS,

            Job.SMN => Role.DPS,
            Job.RDM => Role.DPS,
            Job.BLM => Role.DPS,
            Job.BLU => Role.DPS,

            Job.ACN => Role.DPS,
            Job.THM => Role.DPS,
            _ => Role.Other,
        };
    }

    public static string GetEmoji(Role role)
    {
        Dictionary<Role, string> dict = new Dictionary<Role, string>()
        {
            { Role.Tank, "<:tank:1227407436709761095>" },
            { Role.Healer, "<:healer:1227407228789592114>" },
            { Role.DPS, "<:dps:1227407135537631282>" },
            { Role.Tank | Role.Healer, "<:tankhealer:1227407463498649631>" },
            { Role.Tank | Role.DPS, "<:tankdps:1227407462718509107>" },
            { Role.Healer | Role.DPS, "<:healerdps:1227407229901082736>" },
            { Role.Tank | Role.Healer | Role.DPS, "<:tankhealerdps:1227407465142947930>" },
        };

        return dict.TryGetValue(role, out string? emoji) ? emoji : "<:none:1227411038367125637>";

        //if (dict.TryGetValue(role & (Role.Tank | Role.Healer | Role.DPS), out string emoji))
        //{
        //    return emoji;
        //}
        //else
        //{
        //    return "<:none:1227411038367125637>";
        //}
    }


}