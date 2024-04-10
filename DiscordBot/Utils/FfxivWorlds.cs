namespace DiscordBot.Utils;

public static class FfxivWorlds
{
    public const string REGION_NA = "North America";
    public const string REGION_OCE = "Oceania";
    public const string REGION_EU = "Europe";
    public const string REGION_JPN = "Japan";

    public const string DC_AETHER = "Aether";
    public const string DC_PRIMAL = "Primal";
    public const string DC_CRYSTAL = "Crystal";
    public const string DC_DYNAMIS = "Dynamis";
    public const string DC_MATERIA = "Materia";
    public const string DC_CHAOS = "Chaos";
    public const string DC_LIGHT = "Light";
    public const string DC_ELEMENTAL = "Elemental";
    public const string DC_GAIA = "Gaia";
    public const string DC_MANA = "Mana";
    public const string DC_METEOR = "Meteor";

    public const string WORLD_ADAMANTOISE = "Adamantoise";
    public const string WORLD_CACTUAR = "Cactuar";
    public const string WORLD_FAERIE = "Faerie";
    public const string WORLD_GILGAMESH = "Gilgamesh";
    public const string WORLD_JENOVA = "Jenova";
    public const string WORLD_MIDGARDSORMR = "Midgardsormr";
    public const string WORLD_SARGATANAS = "Sargatanas";
    public const string WORLD_SIREN = "Siren";
    public const string WORLD_BEHEMOTH = "Behemoth";
    public const string WORLD_EXCALIBUR = "Excalibur";
    public const string WORLD_EXODUS = "Exodus";
    public const string WORLD_FAMFRIT = "Famfrit";
    public const string WORLD_HYPERION = "Hyperion";
    public const string WORLD_LAMIA = "Lamia";
    public const string WORLD_LEVIATHAN = "Leviathan";
    public const string WORLD_ULTROS = "Ultros";
    public const string WORLD_BALMUNG = "Balmung";
    public const string WORLD_BRYNHILDR = "Brynhildr";
    public const string WORLD_COEURL = "Coeurl";
    public const string WORLD_DIABOLOS = "Diabolos";
    public const string WORLD_GOBLIN = "Goblin";
    public const string WORLD_MALBORO = "Malboro";
    public const string WORLD_MATEUS = "Mateus";
    public const string WORLD_ZALERA = "Zalera";
    public const string WORLD_HALICARNASSUS = "Halicarnassus";
    public const string WORLD_MADUIN = "Maduin";
    public const string WORLD_MARILITH = "Marilith";
    public const string WORLD_SERAPH = "Seraph";
    public const string WORLD_CERBERUS = "Cerberus";
    public const string WORLD_LOUISOIX = "Louisoix";
    public const string WORLD_MOOGLE = "Moogle";
    public const string WORLD_OMEGA = "Omega";
    public const string WORLD_PHANTOM = "Phantom";
    public const string WORLD_RAGNAROK = "Ragnarok";
    public const string WORLD_SAGITTARIUS = "Sagittarius";
    public const string WORLD_SPRIGGAN = "Spriggan";
    public const string WORLD_ALPHA = "Alpha";
    public const string WORLD_LICH = "Lich";
    public const string WORLD_ODIN = "Odin";
    public const string WORLD_PHOENIX = "Phoenix";
    public const string WORLD_RAIDEN = "Raiden";
    public const string WORLD_SHIVA = "Shiva";
    public const string WORLD_TWINTANIA = "Twintania";
    public const string WORLD_ZODIARK = "Zodiark";
    public const string WORLD_BISMARK = "Bismark";
    public const string WORLD_RAVANA = "Ravana";
    public const string WORLD_SEPHIROT = "Sephirot";
    public const string WORLD_SOPHIA = "Sophia";
    public const string WORLD_ZURVAN = "Zurvan";
    public const string WORLD_AEGIS = "Aegis";
    public const string WORLD_ATAMOS = "Atamos";
    public const string WORLD_CARBUNCLE = "Carbuncle";
    public const string WORLD_GARUDA = "Garuda";
    public const string WORLD_GUNGNIR = "Gungnir";
    public const string WORLD_KUJATA = "Kujata";
    public const string WORLD_TONBERRY = "Tonberry";
    public const string WORLD_ALEXANDER = "Alexander";
    public const string WORLD_BAHAMUT = "Bahamut";
    public const string WORLD_DURANDAL = "Durandal";
    public const string WORLD_FENRIR = "Fenrir";
    public const string WORLD_IFRIT = "Ifrit";
    public const string WORLD_RIDILL = "Ridill";
    public const string WORLD_TIAMAT = "Tiamat";
    public const string WORLD_ULTIMA = "Ultima";
    public const string WORLD_ANIMA = "Anima";
    public const string WORLD_ASURA = "Asura";
    public const string WORLD_CHOCOBO = "Chocobo";
    public const string WORLD_HADES = "Hades";
    public const string WORLD_IXION = "Ixion";
    public const string WORLD_MASAMUNE = "Masamune";
    public const string WORLD_PANDAEMONIUM = "Pandaemonium";
    public const string WORLD_TITAN = "Titan";
    public const string WORLD_BELIAS = "Belias";
    public const string WORLD_MANDRAGORA = "Mandragora";
    public const string WORLD_RAMUH = "Ramuh";
    public const string WORLD_SHINRYU = "Shinryu";
    public const string WORLD_UNICORN = "Unicorn";
    public const string WORLD_VALEFOR = "Valefor";
    public const string WORLD_YOJIMBO = "Yojimbo";
    public const string WORLD_ZEROMUS = "Zeromus";

    private static readonly string[] Regions = { REGION_NA, REGION_OCE, REGION_EU, REGION_JPN };

    private static readonly Dictionary<string, string[]> DataCenterMap = new()
    {
        { REGION_NA, new string[] { DC_AETHER, DC_PRIMAL, DC_CRYSTAL, DC_DYNAMIS } },
        { REGION_OCE, new string[] { DC_MATERIA } },
        { REGION_EU, new string[] { DC_CHAOS, DC_LIGHT } },
        { REGION_JPN, new string[] { DC_ELEMENTAL, DC_GAIA, DC_MANA, DC_METEOR } },
    };

    private static readonly Dictionary<string, string[]> WorldMap = new()
    {
        { DC_AETHER, new string[] { WORLD_ADAMANTOISE, WORLD_CACTUAR, WORLD_FAERIE, WORLD_GILGAMESH, WORLD_JENOVA, WORLD_MIDGARDSORMR, WORLD_SARGATANAS, WORLD_SIREN } },
        { DC_PRIMAL, new string[] { WORLD_BEHEMOTH, WORLD_EXCALIBUR, WORLD_EXODUS, WORLD_FAMFRIT, WORLD_HYPERION, WORLD_LAMIA, WORLD_LEVIATHAN, WORLD_ULTROS } },
        { DC_CRYSTAL, new string[] { WORLD_BALMUNG, WORLD_BRYNHILDR, WORLD_COEURL, WORLD_DIABOLOS, WORLD_GOBLIN, WORLD_MALBORO, WORLD_MATEUS, WORLD_ZALERA } },
        { DC_DYNAMIS, new string[] { WORLD_HALICARNASSUS, WORLD_MADUIN, WORLD_MARILITH, WORLD_SERAPH } },
        { DC_CHAOS, new string[] { WORLD_CERBERUS, WORLD_LOUISOIX, WORLD_MOOGLE, WORLD_OMEGA, WORLD_PHANTOM, WORLD_RAGNAROK, WORLD_SAGITTARIUS, WORLD_SPRIGGAN } },
        { DC_LIGHT, new string[] { WORLD_ALPHA, WORLD_LICH, WORLD_ODIN, WORLD_PHOENIX, WORLD_RAIDEN, WORLD_SHIVA, WORLD_TWINTANIA, WORLD_ZODIARK } },
        { DC_MATERIA, new string[] { WORLD_BISMARK, WORLD_RAVANA, WORLD_SEPHIROT, WORLD_SOPHIA, WORLD_ZURVAN } },
        { DC_ELEMENTAL, new string[] { WORLD_AEGIS, WORLD_ATAMOS, WORLD_CARBUNCLE, WORLD_GARUDA, WORLD_GUNGNIR, WORLD_KUJATA, WORLD_TONBERRY } },
        { DC_GAIA, new string[] { WORLD_ALEXANDER, WORLD_BAHAMUT, WORLD_DURANDAL, WORLD_FENRIR, WORLD_IFRIT, WORLD_RIDILL, WORLD_TIAMAT, WORLD_ULTIMA } },
        { DC_MANA, new string[] { WORLD_ANIMA, WORLD_ASURA, WORLD_CHOCOBO, WORLD_HADES, WORLD_IXION, WORLD_MASAMUNE, WORLD_PANDAEMONIUM, WORLD_TITAN } },
        { DC_METEOR, new string[] { WORLD_BELIAS, WORLD_MANDRAGORA, WORLD_RAMUH, WORLD_SHINRYU, WORLD_UNICORN, WORLD_VALEFOR, WORLD_YOJIMBO, WORLD_ZEROMUS } },
    };

    public static string[] GetWorldsFor(params string[] dataCenters) =>
        dataCenters.SelectMany(dataCenter => WorldMap.ContainsKey(dataCenter) ? WorldMap[dataCenter] : Array.Empty<string>()).ToArray();

    public static string[] GetDataCentersFor(params string[] regions) =>
        regions.SelectMany(region => DataCenterMap.ContainsKey(region) ? DataCenterMap[region] : Array.Empty<string>()).ToArray();

    public static string[] GetRegions() =>
        Regions;

    public static string GetRegionForDataCenter(string dataCenter) =>
        dataCenter == null ? null : DataCenterMap.FirstOrDefault(kv => kv.Value.Contains(dataCenter, StringComparer.InvariantCultureIgnoreCase)).Key;
}
