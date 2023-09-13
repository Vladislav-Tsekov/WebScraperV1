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

            if (titleNodes != null)
            {
                foreach (var titleNode in titleNodes)
                {
                    string title = titleNode.InnerText;

                    string makePattern = @"^(.*?)(?:\s+|$)";
                    string modelPattern = @"(?:\s+|^)(.*?)(?:\s+\d+|$)";
                    string ccPattern = @"(?:\s+|^)(\d{3})(?:\s+|$)";

                    Match makeMatch = Regex.Match(title, makePattern);
                    Match modelMatch = Regex.Match(title, modelPattern);
                    Match ccMatch = Regex.Match(title, ccPattern);

                    string make = makeMatch.Success ? makeMatch.Groups[1].Value.Trim() : "N/A";
                    string model = modelMatch.Success ? modelMatch.Groups[1].Value.Trim() : "N/A";
                    string cc = ccMatch.Success ? ccMatch.Groups[1].Value.Trim() : "N/A";

                    if (string.IsNullOrEmpty(make))
                    {
                        make = "N/A";
                    }

                    model = model.Replace(make, "").Trim();
                    int firstSpaceIndex = model.IndexOf(" ");
                    if (firstSpaceIndex >= 0)
                    {
                        model = model.Substring(0, firstSpaceIndex);
                    }

                    if (string.IsNullOrEmpty(model))
                    {
                        model = "N/A";
                    }

                    var motorcycle = new Models(make, model, cc);

                    allMotorcycles.Add(motorcycle);
                }
            }
            else
            {
                Console.WriteLine("No titles found on this page.");
            }

            var priceNodes = doc.DocumentNode.SelectNodes("//span[@class='price']");

            if (priceNodes != null)
            {
                foreach (var priceNode in priceNodes)
                {
                    string priceInnerText = priceNode.InnerText;
                    string price = Regex.Replace(priceInnerText, @"[^\d]", "");
                    if (int.TryParse(price, out int convertedPrice))
                    {
                        allMotorcycles.Last().Price = convertedPrice;
                        Console.WriteLine(convertedPrice);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to parse price: {priceInnerText}");
                    }
                }
            }
            else
            {
                Console.WriteLine("No prices found on this page.");
            }

            var infoNodes = doc.DocumentNode.SelectNodes("//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]");

            if (infoNodes != null)
            {
                foreach (var infoNode in infoNodes)
                {
                    string infoText = infoNode.InnerText;
                    string yearPattern = @"\d{4}";
                    Match yearMatch = Regex.Match(infoText, yearPattern);

                    if (yearMatch.Success && int.TryParse(yearMatch.Value, out int convertedYear))
                    {
                        allMotorcycles.Last().Year = convertedYear;
                    }
                }
            }
            else
            {
                Console.WriteLine("No years found on this page.");
            }
        }
        else
        {
            Console.WriteLine("No titles found on this page.");
        }

        var priceNodes = doc.DocumentNode.SelectNodes("//span[@class='price']");

        if (priceNodes != null)
        {
            foreach (var priceNode in priceNodes)
            {
                string priceInnerText = priceNode.InnerText;
                string price = Regex.Replace(priceInnerText, @"[^\d]", "");
                if (int.TryParse(price, out int convertedPrice))
                {
                    allMotorcycles.Last().Price = convertedPrice;
                    Console.WriteLine(convertedPrice);
                }
                else
                {
                    Console.WriteLine($"Failed to parse price: {priceInnerText}");
                }
            }
        }
        else
        {
            Console.WriteLine("No prices found on this page.");
        }

        var infoNodes = doc.DocumentNode.SelectNodes("//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]");

        if (infoNodes != null)
        {
            foreach (var infoNode in infoNodes)
            {
                string infoText = infoNode.InnerText;
                string yearPattern = @"\d{4}";
                Match yearMatch = Regex.Match(infoText, yearPattern);

                if (yearMatch.Success && int.TryParse(yearMatch.Value, out int convertedYear))
                {
                    allMotorcycles.Last().Year = convertedYear;
                }
            }
        }
        else
        {
            Console.WriteLine("No years found on this page.");
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

        // Other helper methods for data extraction and validation...
    }

