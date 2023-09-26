using BaseScraper;
using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

class Scraper
{
    static async Task Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        List<string> motorcycleMake = new();
        List<string> motorcyclePrice = new();
        List<string> motorcycleYear = new();

        using HttpClient client = new();
        try
        {
            string baseURL = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=twgj6h&f1=";
            int maxPages = 1; // Change the number according to the pages count

            for (int i = 1; i <= maxPages; i++)
            {
                string currentPageURL = baseURL + i;

                HttpResponseMessage response = await client.GetAsync(currentPageURL);

                if (response.IsSuccessStatusCode)
                {
                    Stream contentStream = await response.Content.ReadAsStreamAsync();

                    using StreamReader reader = new(contentStream, Encoding.GetEncoding("windows-1251"));
                    string htmlContent = await reader.ReadToEndAsync();
                    HtmlDocument doc = new();
                    doc.LoadHtml(htmlContent);

                    var titleNodes = doc.DocumentNode.SelectNodes("//a[@class='mmm']");
                    var priceNodes = doc.DocumentNode.SelectNodes("//span[@class='price']");
                    var descriptionNodes = doc.DocumentNode.SelectNodes("//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]");

                    // HOW TO SEPARATE INTO SMALLER CATEGORIES

                    //string makePattern = @"^(.*?)(?:\s+|$)";
                    //string modelPattern = @"(?:\s+|^)(.*?)(?:\s+\d+|$)";
                    //string ccPattern = @"(?:\s+|^)(\d{3})(?:\s+|$)";

                    if (titleNodes != null)
                    {
                        foreach (var titleNode in titleNodes)
                        {
                            string title = titleNode.InnerText;

                            if (string.IsNullOrEmpty(title))
                            {
                                Console.WriteLine("Empty Title");
                                continue;
                            }
                            else
                            {
                                string[] titleTokens = title.Split();
                                string make = titleTokens[0];
                                Console.WriteLine($"Title: {make}");
                                motorcycleMake.Add(make);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No motorcycle titles found on the page.");
                    }

                    if (priceNodes != null)
                    {
                        foreach (var priceNode in priceNodes)
                        {
                            string priceInnerText = priceNode.InnerText;
                            string price = Regex.Replace(priceInnerText, @"[^\d]", "");
                            Console.WriteLine($"Price: {price}");
                            motorcyclePrice.Add(price);
                        }
                    }
                    else
                    {
                        Console.WriteLine("No motorcycle prices found on the page.");
                    }

                    if (descriptionNodes != null)
                    {
                        foreach (var infoNode in descriptionNodes)
                        {
                            string infoText = infoNode.InnerText;
                            string yearPattern = @"\d{4}";
                            Match yearMatch = Regex.Match(infoText, yearPattern);
                            if (yearMatch.Success)
                            {
                                string year = yearMatch.Value;
                                Console.WriteLine($"Year: {year}");
                                motorcycleYear.Add(year);
                            }
                            else
                            {
                                Console.WriteLine($"No year found!");
                                motorcycleYear.Add("N/A");
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
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }

        List<Motorcycle> motorcycles = new();

        for (int i = 0; i < motorcycleMake.Count; i++)
        {
            Console.WriteLine($"{motorcycleMake[i]} - {motorcycleYear[i]} - {motorcyclePrice[i]}");
            Motorcycle motorcycle = new(motorcycleMake[i], motorcycleYear[i], motorcyclePrice[i]);
            motorcycles.Add(motorcycle);
        }

        using StreamWriter mxWriter = new(@"../../../MotoData.csv");

        foreach (var motorcycle in motorcycles)
        {
            mxWriter.Write($"{motorcycle.Title}, {motorcycle.Year}, {motorcycle.Price}{Environment.NewLine}");
        }
    }
}