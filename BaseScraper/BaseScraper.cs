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
        List<string> motorcycleCC = new();
        List<double> motorcyclePrice = new();
        List<string> motorcycleYear = new();

        using HttpClient client = new();
        try
        {
            // Find a work-around since the filter is not stored on the server and changes over time
            string baseURL = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=twgj6h&f1=";

            // Change the number according to the pages count
            int maxPages = 21; 

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

                    if (titleNodes != null)
                    {
                        foreach (var titleNode in titleNodes)
                        {
                            string title = titleNode.InnerText;

                            if (string.IsNullOrEmpty(title))
                            {
                                continue;
                            }
                            else
                            {
                                string[] titleTokens = title.Split();

                                string make = titleTokens[0];
                                motorcycleMake.Add(make);

                                string cc = "N/A";

                                foreach (string cubicCent in titleTokens)
                                {
                                    Match ccMatch = Regex.Match(cubicCent, @"\d{3}");
                                    if (ccMatch.Success)
                                    {
                                        string ccValue = ccMatch.Value;
                                        cc = ccValue;
                                        motorcycleCC.Add(cc);
                                    }
                                }

                                if (cc == "N/A")
                                {
                                    motorcycleCC.Add(cc);
                                }
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

                            if (double.TryParse(price, out double priceValue))
                            {
                                motorcyclePrice.Add(priceValue);
                            }
                            else
                            {
                                motorcyclePrice.Add(0);
                            }
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
                                motorcycleYear.Add(year);
                            }
                            else
                            {
                                motorcycleYear.Add("N/A");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("No motorcycle years found on the page.");
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
            Motorcycle motorcycle = new(motorcycleMake[i], motorcycleCC[i], motorcycleYear[i], motorcyclePrice[i]);
            motorcycles.Add(motorcycle);
        }

        List<Motorcycle> sortedMoto = motorcycles.OrderBy(m => m.Make).ThenBy(m => m.Year).ThenBy(m => m.Price).ToList();

        using StreamWriter mxWriter = new(@"../../../MotoData.csv");

        mxWriter.Write($"Make, CC, Year, Price{Environment.NewLine}");

        foreach (var motorcycle in sortedMoto)
        {
            mxWriter.Write($"{motorcycle.Make}, {motorcycle.CC}, {motorcycle.Year}, {motorcycle.Price}{Environment.NewLine}");
        }

        mxWriter.Dispose();

        var averagePrices = sortedMoto
            .GroupBy(m => new { m.Make, m.Year }) // Group by year and make
            .Select(group => new
            {
                group.Key.Make,
                group.Key.Year,
                AveragePrice = group.Average(m => m.Price)
            })
            .OrderBy(m => m.Make)
            .ThenBy(m => m.Year)
            .ThenBy(m => m.AveragePrice);

        using StreamWriter avgPriceWriter = new(@"../../../AvgPriceModelYear.csv");

        avgPriceWriter.Write($"Make, Year, Average Price{Environment.NewLine}");

        foreach (var moto in averagePrices)
        {
            avgPriceWriter.Write($"{moto.Make}, {moto.Year}, {moto.AveragePrice:f2}{Environment.NewLine}");
        }

        avgPriceWriter.Dispose();
    }
}