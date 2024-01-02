namespace BaseScraper.Config;

public static class StringsConstants
{
    //Config
    public const string AppSettingsPath = "appsettings.json";

    //Base URLs
    public const string MxBaseUrl = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=uy15oi&f1=";
    public const string EnduroBaseUrl = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=umpnpb&f1=";

    //Encoding
    public const string Encoding = "windows-1251";

    //Nodes
    public const string TitleNodes = "//a[@class='mmm']";
    public const string PriceNodes = "//span[@class='price']";
    public const string DescriptionNodes = "//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]";
    public const string LinkNodes = "//a[@class='mmm']";
    public const string HrefAttribute = "href";
    public const string HrefDefault = "";

    //Output
    public const string YearIsNull = "0";
    public const string NoTitlesFound = "No titles found on the page.";
    public const string NoPricesFound = "No prices found on the page.";
    public const string NoYearsFound = "No years found on the page.";
    public const string NoLinksFound = "No matching links found.";
    public const string FailedToRetreivePage = "Failed to retrieve the web page.";

    //RegEx
    public const string CcPattern = @"\d{3}";
    public const string PriceIdentify = @"[^\d]";
    public const string PriceReplace = "";
    public const string YearPattern = @"\d{4}";
}
