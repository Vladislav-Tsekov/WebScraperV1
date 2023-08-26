using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;
using WebScraper;

class Scraper
{
    static async Task Main(string[] args)
    {
        // Register 'windows-1251' encoding provider in order to read Cyrillic
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        List<string[]> motorcycleTitles = new();
        List<int> motorcycleYears = new();
        List<int> motorcyclePrices = new();

        using HttpClient client = new();
        try
        {
            // BASE URL IS USED BECAUSE THE URL STRUCTURE IS SIMILAR AND ONLY THE LAST SYMBOL (COUNTER) CHANGES
            string baseURL = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=thgxfy&f1=";
            
            // MAX NUMBER OF PAGES TO SCRAPE
            int maxPages = 9; // ADJUST THIS IF NEEDED, CURRENT NUMBER = 9 (26.08.2023)

            for (int i = 0; i < maxPages; i++)
            {
                string currentPageURL = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=thgxfy&f1=" + i;

                // Send an HTTP request to the URL
                HttpResponseMessage response = await client.GetAsync(currentPageURL);

                if (response.IsSuccessStatusCode)
                {
                    Stream contentStream = await response.Content.ReadAsStreamAsync();

                    // Read the stream with the correct encoding (windows-1251 for Cyrillic)
                    using StreamReader reader = new(contentStream, Encoding.GetEncoding("windows-1251"));

                    string htmlContent = await reader.ReadToEndAsync();

                    // Parse the HTML content
                    HtmlDocument doc = new();
                    doc.LoadHtml(htmlContent);

                    // Extract the motorcycle title using XPath
                    var titleNodes = doc.DocumentNode.SelectNodes("//a[@class='mmm']");

                    if (titleNodes != null)
                    {
                        foreach (var titleNode in titleNodes)
                        {
                            string title = titleNode.InnerText;

                            // Defined regex patterns to capture the make, model and cc - separately

                            string makePattern = @"^(.*?)(?:\s+|$)"; // Find the make (Ktm, Yamaha, etc)
                            string modelPattern = @"(?:\s+|^)(.*?)(?:\s+\d+|$)"; // Find the model (EXC, WR, etc)
                            string ccPattern = @"(?:\s+|^)(\d+)?(?:\s+|$)"; // Find the engine size (250, 300, etc)

                            Match makeMatch = Regex.Match(title, makePattern);
                            Match modelMatch = Regex.Match(title, modelPattern);
                            Match ccMatch = Regex.Match(title, ccPattern);

                            string make = makeMatch.Success ? makeMatch.Groups[1].Value.Trim() : "";
                            string model = modelMatch.Success ? modelMatch.Groups[1].Value.Trim() : "";
                            string cc = ccMatch.Success ? ccMatch.Groups[1].Value.Trim() : "N/A";

                            if (string.IsNullOrEmpty(make))
                            {
                                continue;
                            }

                            model = model.Replace(make, "").Trim();
                            int firstSpaceIndex = model.IndexOf(" ");
                            if (firstSpaceIndex >= 0)
                            {
                                model = model.Substring(0, firstSpaceIndex);
                            }

                            string[] motoTitle = { make, model, cc };

                            motorcycleTitles.Add(motoTitle);
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format(Utilities.NoTitleFound));
                    }

                    // Extract the motorcycle prices using XPath
                    var priceNodes = doc.DocumentNode.SelectNodes("//span[@class='price']");

                    if (priceNodes != null)
                    {
                        foreach (var priceNode in priceNodes)
                        {
                            string priceInnerText = priceNode.InnerText;
                            string price = Regex.Replace(priceInnerText, @"[^\d]", ""); // SHOULD REMOVE ".ЛВ" AT THE END
                            //Console.WriteLine($"Price: {price}");
                            int convertedPrice = int.Parse(price);
                            motorcyclePrices.Add(convertedPrice);
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format(Utilities.NoPriceFound));
                    }

                    // Extract the motorcycle year using XPath and regular expressions
                    var infoNodes = doc.DocumentNode.SelectNodes("//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]");

                    if (infoNodes != null)
                    {
                        foreach (var infoNode in infoNodes)
                        {
                            string infoText = infoNode.InnerText;
                            string yearPattern = @"\d{4}";
                            Match yearMatch = Regex.Match(infoText, yearPattern);

                            if (yearMatch.Success)
                            {
                                string year = yearMatch.Value;
                                int convertedYear = int.Parse(year);
                                //Console.WriteLine($"Year: {year}");
                                motorcycleYears.Add(convertedYear);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(string.Format(Utilities.NoYearFound));
                    }
                }
                else
                {
                    Console.WriteLine(string.Format(Utilities.WebpageFailed));
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        List<Motorcycle> currPageMotorcycles = new();

        // Print command for testing the output and merging collections

        for (int i = 0; i < motorcycleTitles.Count; i++)
        {
            var currentMotorcycle = new Motorcycle(motorcycleTitles[i][0], motorcycleTitles[i][1], motorcycleTitles[i][2], motorcycleYears[i], motorcyclePrices[i]);

            currPageMotorcycles.Add(currentMotorcycle);

            Console.WriteLine($"{i+1}. Make: {motorcycleTitles[i][0]}, Model: {motorcycleTitles[i][1]}, CC: {motorcycleTitles[i][2]}, Year: {motorcycleYears[i]}, Price: {motorcyclePrices[i]}");
        }
    }
}