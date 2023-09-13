using HtmlAgilityPack;
using System.Text.RegularExpressions;
using TestScrapeV1._1;

public class WebScraper
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=tr9len&f1=";
    private const int MaxPages = 2; // Adjust this if needed

    public WebScraper()
    {
        var httpClient = new HttpClient();
        // HTTP CONFIG - USER-AGENT / T.O. / ETC...
    }

    public async Task BeginScraping()
    {
        List<Models> allMotorcycles = new List<Models>();

        for (int i = 0; i < MaxPages; i++)
        {
            string currentPageUrl = BaseUrl + i;
            try
            {
                string htmlContent = await FetchPageAsync(currentPageUrl);
                HtmlDocument doc = ParseHtml(htmlContent);
                ExtractDataFromPage(doc, allMotorcycles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred on page {i}: {ex.Message}");
                // Handle the error gracefully
            }
        }

        ExportDataToTextFile(allMotorcycles);

        Console.WriteLine($"Scraping has ended, {allMotorcycles.Count} motorcycles were scraped successfully!");
    }

    private async Task<string> FetchPageAsync(string url)
    {
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private HtmlDocument ParseHtml(string htmlContent)
    {
        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(htmlContent);
        return doc;
    }

    private void ExtractDataFromPage(HtmlDocument doc, List<Models> allMotorcycles)
    {
        var titleNodes = doc.DocumentNode.SelectNodes("//a[@class='mmm']");
        var priceNodes = doc.DocumentNode.SelectNodes("//span[@class='price']");
        var infoNodes = doc.DocumentNode.SelectNodes("//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]");

        if (titleNodes != null && priceNodes != null && infoNodes != null)
        {
            List<Models> motorcycleDataList = new List<Models>();

            // Scrape and collect data
            foreach (var titleNode in titleNodes)
            {
                string title = titleNode.InnerText;
                string makePattern = @"^(.*?)(?:\s+|$)";
                string modelPattern = @"(?:\s+|^)(.*?)(?:\s+\d+|$)";
                string ccPattern = @"(?:\s+|^)(\d{3})(?:\s+|$)";

                var priceNode = titleNode.SelectSingleNode("../following-sibling::span[@class='price']");
                var infoNode = titleNode.SelectSingleNode("../../following-sibling::tr/td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]");

                string priceInnerText = priceNode.InnerText;
                string price = Regex.Replace(priceInnerText, @"[^\d]", "");
                string infoText = infoNode.InnerText;
                string yearPattern = @"\d{4}";

                Match makeMatch = Regex.Match(title, makePattern);
                Match modelMatch = Regex.Match(title, modelPattern);
                Match ccMatch = Regex.Match(title, ccPattern);
                Match yearMatch = Regex.Match(infoText, yearPattern);

                string make = makeMatch.Success ? makeMatch.Groups[1].Value.Trim() : "N/A";
                string model = modelMatch.Success ? modelMatch.Groups[1].Value.Trim() : "N/A";
                string cc = ccMatch.Success ? ccMatch.Groups[1].Value.Trim() : "N/A";
                int year = yearMatch.Success && int.TryParse(yearMatch.Value, out int convertedYear) ? convertedYear : -1;

                int convertedPrice = -1;

                if (int.TryParse(price, out convertedPrice))
                {
                    motorcycleDataList.Add((make, model, cc, year, convertedPrice));
                }
                else
                {
                    Console.WriteLine($"Failed to parse price: {priceInnerText}");
                }
            }

            // Create Motorcycle objects
            foreach (var motorcycleData in motorcycleDataList)
            {
                var motorcycle = new Models(motorcycleData.Make, motorcycleData.Model, motorcycleData.CC, motorcycleData.Year, motorcycleData.Price);
                allMotorcycles.Add(motorcycle);
            }
        }
        else
        {
            Console.WriteLine("Some data not found on this page.");
        }
    }

    private void ExportDataToTextFile(List<Models> motorcycles)
    {
        using StreamWriter writer = new StreamWriter(@"../../../Motocross.txt");

        foreach (var motorcycle in motorcycles)
        {
            writer.Write($"{motorcycle.Make}, {motorcycle.Model}, {motorcycle.CC}, {motorcycle.Year}, {motorcycle.Price}{Environment.NewLine}");
        }
    }

}

