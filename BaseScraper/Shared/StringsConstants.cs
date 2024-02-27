using System.Linq;

namespace BaseScraper.Config;

public static class StringsConstants
{
    //Config
    public const string AppSettingsPath = "appsettings.json";

    //Base URLs
    public const string MxBaseUrl = "https://www.mobile.bg/obiavi/motori/krosov/ot-2006/do-2024/p-";
    public const string EnduroBaseUrl = "";

    //Encoding
    public const string Encode = "windows-1251";

    //Nodes
    public const string TitleNodes = "//a[@class='mmmL']";
    public const string PriceNodes = "//span[@class='price']";
    public const string DescriptionNodes = "//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]";
    public const string LinkNodes = "//a[@class='photoLinkL']"; 
    public const string HrefAttribute = "href";
    public const string HrefDefault = "";

    //Base Scraper
    public const int YearIsNull = 0;
    public const int CcIsNull = 0;
    public const string NoTitlesFound = "No titles found on the page.";
    public const string NoPricesFound = "No prices found on the page.";
    public const string NoYearsFound = "No years found on the page.";
    public const string NoLinksFound = "No matching links found.";
    public const string FailedToRetreivePage = "Failed to retrieve the web page.";

    //StreamWriters
    //DataExport => AddMotorcycleEntries
    public const string AllLinksTitles = "Make, CC, Year, Price, Link";
    public const string AllLinksMotoInfo = "{0},{1},{2},{3},{4}";
    public const string AllLinksPriceChange = "Updating {0}. Old Price: {1:0}, New Price: {2:0}";
    public const string AllLinksEntryExists = "Entry with link {0} already exists, and prices match.";
    public const string MarketPriceTitles = "Make, Year, Count, Average Price, Mean Price, StdDev Price, Median Price, Price Variance";
    public const string MarketPriceMotoInfo = "{0}, {1}, {2}, {3:0}, {4:0}, {5:0}, {6:0}, {7:f3}";

    //CSV File Names
    public const string AllLinksCsv = "AllMotocrossLinks.csv";
    public const string MarketPriceCsv = "AvgPriceMotocross.csv";
    //DataAnalysis => MarketOverviewReport
    public const string MarketOverviewCsv = "MarketOverview.csv";
    //DataAnalysis => SoldMotorcyclesReport
    public const string SaleReportCsv = "SaleReport.csv";
    //DataAnalysis => UnusualValuesReport
    public const string MarketOutliersCsv = "MarketOutliers.csv";

    //Regular Expressions (RegEx)
    public const string CcPattern = @"\d{3}";
    public const string PriceIdentify = @"[^\d]";
    public const string PriceReplace = "";
    public const string YearPattern = @"\d{4}";
}
