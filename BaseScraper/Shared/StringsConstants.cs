using System.Linq;

namespace BaseScraper.Config;

public static class StringsConstants
{
    //Config
    public const string AppSettingsPath = "appsettings.json";

    //Base URLs
    public const string MxBaseUrl = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=v7i8sj&f1=";
    public const string EnduroBaseUrl = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=umpnpb&f1=";

    //Encoding
    public const string Encode = "windows-1251";

    //Nodes
    public const string TitleNodes = "//a[@class='mmm']";
    public const string PriceNodes = "//span[@class='price']";
    public const string DescriptionNodes = "//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]";
    public const string LinkNodes = "//a[@class='mmm']";
    public const string HrefAttribute = "href";
    public const string HrefDefault = "";

    //Output - Base Scraper
    public const string YearIsNull = "0";
    public const string NotAvailable = "N/A";
    public const string NoTitlesFound = "No titles found on the page.";
    public const string NoPricesFound = "No prices found on the page.";
    public const string NoYearsFound = "No years found on the page.";
    public const string NoLinksFound = "No matching links found.";
    public const string FailedToRetreivePage = "Failed to retrieve the web page.";

    //StreamWriters
    //DataExport => AddMotorcycleEntries
    public const string AllLinksCsv = "AllMotocrossLinks.csv";
    public const string AllLinksTitles = "Make, CC, Year, Price, Link";
    public const string AllLinksMotoInfo = "{0},{1},{2},{3},{4}";
    public const string AllLinksPriceChange = "Updating {0}. Old Price: {1}, New Price: {2}";
    public const string AllLinksEntryExists = "Entry with link {0} already exists, and prices match.";

    //RegEx
    public const string CcPattern = @"\d{3}";
    public const string PriceIdentify = @"[^\d]";
    public const string PriceReplace = "";
    public const string YearPattern = @"\d{4}";
}
