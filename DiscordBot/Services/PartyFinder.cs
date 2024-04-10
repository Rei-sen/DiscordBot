using Discord;
using Discord.Interactions;
using HtmlAgilityPack;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace DiscordBot.Services;

public class PartyFinder : InteractionModuleBase<SocketInteractionContext>, IHostedService
{
    public class PartyFinderListing
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

    private readonly HttpClient _httpClient = new()
    {
        BaseAddress = new Uri("https://xivpf.com"),
    };
    private List<PartyFinderListing> _listings = new List<PartyFinderListing>();
    private Timer _timer;

    public PartyFinder()
    {
        //UpdateLatestPartyFinderListings().GetAwaiter().GetResult();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
       _timer = new Timer(async (_) => await UpdateLatestPartyFinderListings(), null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _timer?.Dispose();
        return Task.CompletedTask;
    }

    public enum DataCenters
    {
        Aether,
        Chaos,
        Crystal,
        Dynamis,
        Elemental,
        Gaia,
        Light,
        Mana,
        Materia,
        Meteor,
        Primal,
    }
    public enum Job
    {
        PLD,
        WAR,
        DRK,
        GNB,

        WHM,
        SCH,
        AST,
        SGE,

        MNK,
        DRG,
        NIN,
        SAM,
        RPR,

        BRD,
        MCH,
        DNC,

        SMN,
        RDM,
        BLM,
        BLU,

        CRP,
        BSM,
        ARM,
        GSM,
        LTW,
        WVR,
        ALC,
        CUL,

        MIN,
        BTN,
        FSH,

        GLD,
        MRD,

        CNJ,

        PGL,
        LNC,
        ROG,

        ARC,

        ACN,
        THM,
        Unknown,
    }

    public static class JobExtensions
    {
        public static Job JobStringToJob(string s)
        {
            return s switch
            {
                "PLD" => Job.PLD,
                "WAR" => Job.WAR,
                "DRK" => Job.DRK,
                "GNB" => Job.GNB,

                "WHM" => Job.WHM,
                "SCH" => Job.SCH,
                "AST" => Job.AST,
                "SGE" => Job.SGE,

                "MNK" => Job.MNK,
                "DRG" => Job.DRG,
                "NIN" => Job.NIN,
                "SAM" => Job.SAM,
                "RPR" => Job.RPR,

                "BRD" => Job.BRD,
                "MCH" => Job.MCH,
                "DNC" => Job.DNC,

                "SMN" => Job.SMN,
                "RDM" => Job.RDM,
                "BLM" => Job.BLM,
                "BLU" => Job.BLU,

                "CRP" => Job.CRP,
                "BSM" => Job.BSM,
                "ARM" => Job.ARM,
                "GSM" => Job.GSM,
                "LTW" => Job.LTW,
                "WVR" => Job.WVR,
                "ALC" => Job.ALC,
                "CUL" => Job.CUL,

                "MIN" => Job.MIN,
                "BTN" => Job.BTN,
                "FSH" => Job.FSH,

                "GLD" => Job.GLD,
                "MRD" => Job.MRD,
                "CNJ" => Job.CNJ,
                "PGL" => Job.PGL,
                "LNC" => Job.LNC,
                "ROG" => Job.ROG,
                "ARC" => Job.ARC,
                "ACN" => Job.ACN,
                "THM" => Job.THM,
                _ => Job.Unknown,
            };
        }

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
            if (_emojiDict.TryGetValue(job, out string emoji))
            {
                return emoji;
            }
            else
            {
                return "<:nonetaken:1227407859285753996>";
            }
        }
    }

    [Flags]
    public enum Role
    {
        Tank = 1,
        Healer = 2,
        DPS = 4,
        Other = 8,
    }

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

            if (dict.TryGetValue(role, out string emoji))
            {
                return emoji;
            }
            else
            {
                return "<:none:1227411038367125637>";
            }
        }


    }
    public struct Slot
    {
        public List<Job> AvailableJobs { get; set; }
        public bool IsFree { get; set; }

        public Slot(List<string> jobs, bool isFree)
        {
            AvailableJobs = jobs.Select(j => JobExtensions.JobStringToJob(j)).ToList();
            IsFree = isFree;
        }

        public string GetEmoji()
        {
            if (IsFree)
            {
                var roleValue = AvailableJobs.Select(j => RoleExtensions.FromJob(j)).Aggregate((a, b) => a | b);
                return RoleExtensions.GetEmoji(roleValue);
            }
            else
            {
                return JobExtensions.GetEmoji(AvailableJobs.FirstOrDefault(Job.Unknown));
            }
        }

    }


    [SlashCommand("subscribetopf", "Subscribe this channel to PF listings")]
    public async Task SubscribeChannelToListings()
    {
        var listings = GetEmbed();

        await RespondAsync(embed: listings, ephemeral: true);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Generates an embed containing the party finder listings.
    /// </summary>
    /// <returns>An Embed object representing the party finder listings.</returns>
    public Embed GetEmbed()
    {
        Console.WriteLine("Generating party finder embed...");
        Console.WriteLine(_listings.Count);
        
        var builder = new EmbedBuilder()
            .WithTitle("PF Listings")
            .WithColor(Color.Blue);
        int maxListings = EmbedBuilder.MaxFieldCount / 3 - 1;
        foreach (var l in _listings.Take(maxListings))
        {
            builder.AddField(l.CreatorName, string.Join(" ", l.PartySlots.Select(s => s.GetEmoji())), true)
                .AddField("tag here", l.Description.Equals("") ? "\u200b" : l.Description, true)
                .AddField($"{l.TimeSinceLastUpdate} ago", $"Expires in {l.TimeUntilExpiration}", true);
        }

        if (_listings.Count - maxListings > 0)
        {
            builder.WithFooter($"{_listings.Count - maxListings} not shown!");
        }

        return builder.Build();
    }

    /// <summary>
    /// Updates the latest party finder listings by making a request to the external website and parsing the HTML content.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task UpdateLatestPartyFinderListings()
    {
        Console.WriteLine("Updating party finder listings...");
        HttpResponseMessage response = await _httpClient.GetAsync("listings");
        string htmlContent = await response.Content.ReadAsStringAsync();

        _listings = ParseHtml(htmlContent);
    }

    /// <summary>
    /// Parses the HTML content and extracts the party finder listings.
    /// </summary>
    /// <param name="htmlContent">The HTML content to parse.</param>
    /// <returns>A list of PartyFinderListing objects representing the extracted party finder listings.</returns>
    private List<PartyFinderListing> ParseHtml(string htmlContent)
    {
        List<PartyFinderListing> listings = new List<PartyFinderListing>();

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);

        HtmlNodeCollection listingNodes = doc.DocumentNode.SelectNodes("//div[@class='listing']");
        if (listingNodes == null)
            return listings;

        foreach (HtmlNode listingNode in listingNodes)
        {
            PartyFinderListing listing = new PartyFinderListing();

            // Parse creator name
            listing.CreatorName = listingNode.SelectSingleNode(".//div[@class='item creator']/span[@class='text']")?.InnerText ?? "Unknown";

            // Parse data center
            listing.DataCenter = listingNode.GetAttributeValue("data-centre", "");

            // Parse category
            listing.Category = listingNode.GetAttributeValue("data-pf-category", "");

            // Parse duty name
            listing.DutyName = WebUtility.HtmlDecode(listingNode.SelectSingleNode("./div[@class='left']/div[@class='duty cross']")?.InnerText) ?? "";

            // Parse description and tag
            listing.Description = WebUtility.HtmlDecode(listingNode.SelectSingleNode("./div[@class='left']/div[@class='description']")?.InnerText.Trim()) ?? "";

            // Parse party slots
            var slotNodes = listingNode.SelectSingleNode("./div[@class='left']/div[@class='party']");
            listing.PartySlots = ParsePartyNode(slotNodes);

            // Parse minimum item level
            string minItemLevelStr = listingNode.SelectSingleNode(".//div[@class='middle']//div[@class='value']")?.InnerText ?? "0";
            uint.TryParse(minItemLevelStr, out uint minItemLevel);
            listing.MinItemLevel = minItemLevel;

            // Parse time until expiration
            string expiresStr = listingNode.SelectSingleNode(".//div[@class='item expires']/span[@class='text']")?.InnerText ?? "0";
            listing.TimeUntilExpiration = ParseTimeSpan(expiresStr);

            // Parse time since last update
            string updatedStr = listingNode.SelectSingleNode(".//div[@class='item updated']/span[@class='text']")?.InnerText ?? "0";
            listing.TimeSinceLastUpdate = ParseTimeSpan(updatedStr);

            listings.Add(listing);
        }

        return listings;
    }


    /// <summary>
    /// Parses the HTML content and extracts the party composition.
    /// </summary>
    /// <param name="partyNode">The HTML node containing the party information.</param>
    /// <returns>A list of Slot objects representing the extracted party slots.</returns>
    private List<Slot> ParsePartyNode(HtmlNode partyNode)
    {
        List<Slot> partySlots = new List<Slot>();

        foreach (HtmlNode slotNode in partyNode.SelectNodes("./div[not(@class='total')]"))
        {
            var isFree = slotNode.GetAttributeValue("class", "").Contains("filled") ? false : true;
            var availableJobs = slotNode.GetAttributeValue("title", "").Split(' ').ToList();
            var slot = new Slot(availableJobs, isFree);

            partySlots.Add(slot);
        }

        return partySlots;
    }

    /// <summary>
    /// Parses the time string and converts it to a TimeSpan object.
    /// </summary>
    /// <param name="timeStr">The time string to parse.</param>
    /// <returns>A TimeSpan object representing the parsed time.</returns>
    private TimeSpan ParseTimeSpan(string timeStr)
    {
        if (timeStr.Equals("in an hour"))
        {
            return TimeSpan.FromHours(1);
        }
        else if (timeStr.Equals("in a minute"))
        {
            return TimeSpan.FromMinutes(1);
        }
        else if (timeStr.StartsWith("in"))
        {
            // Time until expiration format: "in X minutes"
            var split = timeStr.Split(' ');
            int time = int.Parse(split[1]);

            if (split[2].Equals("minutes"))
            {
                return TimeSpan.FromMinutes(time);
            }
            else
            {
                return TimeSpan.FromSeconds(time);
            }
        }
        else if (timeStr.Equals("a minute ago"))
        {
            return TimeSpan.FromMinutes(1);
        }
        else if (timeStr.Equals("an hour ago"))
        {
            return TimeSpan.FromHours(1);
        }
        else if (timeStr.Equals("now"))
        {
            return TimeSpan.Zero;
        }
        else
        {
            // Time since last update format: "X minutes ago"
            var split = timeStr.Split(' ');
            int time = int.Parse(split[0]);
            string unit = split[1];

            if (unit.EndsWith("s"))
            {
                unit = unit[..^1]; // Remove the 's' at the end
            }

            switch (unit)
            {
                case "minute":
                    return TimeSpan.FromMinutes(time);
                case "hour":
                    return TimeSpan.FromHours(time);
                case "second":
                    return TimeSpan.FromSeconds(time);
                default:
                    throw new ArgumentException("Invalid time unit.");
            }
        }
    }
}
