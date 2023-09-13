using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;
class Scraper
{
    static async Task Main(string[] args)
    {
        // Register the 'windows-1251' encoding provider in order to read Cyrillic
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        // URL of the website to scrape
        string url = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=thgxfy&f1=1";
        // Insert a function that finds the "next" button - Web Crawler

        // Creating collections to store the scraped information, at first - three different collections for title, price and year
        List<string> motorcycleTitle = new List<string>();
        List<string> motorcyclePrice = new List<string>();
        List<string> motorcycleYear = new List<string>();

        List<string> motorcycleTitle = new();
        List<string> motorcyclePrice = new();
        List<string> motorcycleYear = new();

        // Create an instance of HttpClient
        using HttpClient client = new();
        try
        {
            // Send an HTTP request to the URL
            HttpResponseMessage response = await client.GetAsync(url);
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
                        if (string.IsNullOrEmpty(title))
                        {
                            continue;
                        }

                        //Console.WriteLine($"Name: {title}");
                        //Console.WriteLine($"Title: {title}");
                        motorcycleTitle.Add(title);
                    }
                }
                else
                {
                    Console.WriteLine("No motorcycle names found on the page.");
                    Console.WriteLine("No motorcycle titles found on the page.");
                }

                // Extract the motorcycle prices using XPath
                var priceNodes = doc.DocumentNode.SelectNodes("//span[@class='price']");
                if (priceNodes != null)
                {
                    foreach (var priceNode in priceNodes)
                    {
                        string price = priceNode.InnerText;
                        string priceInnerText = priceNode.InnerText;
                        string price = Regex.Replace(priceInnerText, @"[^\d]", ""); // SHOULD REMOVE ".ЛВ" AT THE END
                        //Console.WriteLine($"Price: {price}");
                        motorcyclePrice.Add(price);
                    }
                }
                else
                {
                    Console.WriteLine("No motorcycle prices found on the page.");
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
                            //Console.WriteLine($"Year: {year}");
                            motorcycleYear.Add(year);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No motorcycle information found on the page.");
                }
            }
            else
            {
                Console.WriteLine("Failed to retrieve the web page.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        for (int i = 0; i < motorcycleTitle.Count; i++)
        {
            Console.WriteLine($"{motorcycleTitle[i]} - {motorcyclePrice[i]} - {motorcycleYear[i]}");
        }
    }
}