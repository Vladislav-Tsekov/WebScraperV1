using BaseScraper.Models;
using BaseScraper.Calculations;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.RegularExpressions;

namespace BaseScraper
{
    public class Scraper
    {
        public static async Task Main()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath)
                .Build();

            string? outputFolderPath = configuration["OutputFolderPath"];

            List<string> motorcycleMake = new();
            List<string> motorcycleCC = new();
            List<double> motorcyclePrice = new();
            List<string> motorcycleYear = new();
            List<string> announcementLink = new();

            using HttpClient client = new();
            try
            {
                string baseURL = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=ug45d2&f1=";

                //Change the number according to the pages count
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
                        var linkNodes = doc.DocumentNode.SelectNodes("//a[@class='mmm']");

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

                        if (linkNodes != null)
                        {
                            foreach (var href in linkNodes)
                            {
                                string link = href.GetAttributeValue("href", "");

                                if (link.Length < 50)
                                    continue;
                                else
                                    announcementLink.Add(link);
                            }
                        }
                        else
                        {
                            Console.WriteLine("No matching links found.");
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
                Motorcycle motorcycle =
                    new(motorcycleMake[i], motorcycleCC[i], motorcycleYear[i], motorcyclePrice[i], announcementLink[i]);
                motorcycles.Add(motorcycle);
            }

            List<Motorcycle> filteredMoto = motorcycles
                                            .Where(m => m.Price > 3000)
                                            .OrderBy(m => m.Make)
                                            .ThenBy(m => m.Year)
                                            .ThenBy(m => m.Price)
                                            .ToList();

            using StreamWriter mxWriter = new(Path.Combine(outputFolderPath, "MotoData.csv"));

            mxWriter.Write($"Make, CC, Year, Price{Environment.NewLine}");

            foreach (var motorcycle in filteredMoto)
            {
                mxWriter.Write($"{motorcycle.Make}, {motorcycle.CC}, {motorcycle.Year}, {motorcycle.Price}, {motorcycle.Link}{Environment.NewLine}");
            }

            mxWriter.Dispose();

            var averagePrices = filteredMoto
                .GroupBy(m => new { m.Make, m.Year })
                .Select(group => new
                {
                    group.Key.Make,
                    group.Key.Year,
                    AveragePrice = group.Average(m => m.Price),
                    MeanPrice = MeanValues.Mean(group.Select(m => m.Price), MeanValues.trimPercentage),
                    DevPrice = MeanValues.Dev(group.Select(m => m.Price), MeanValues.deviationThreshold),
                    MotorcycleCount = group.Count(),
                })
                .OrderBy(m => m.Make)
                .ThenBy(m => m.Year)
                .ThenBy(m => m.AveragePrice);

            using StreamWriter priceWriter = new(Path.Combine(outputFolderPath, "AvgPriceModelYear.csv"));

            priceWriter.Write($"Make, Year, Average Price, Mean Price, StdDev Price, Combined Price, Count{Environment.NewLine}");

            foreach (var moto in averagePrices)
            {
                double combinedPrice = (moto.AveragePrice + moto.DevPrice + moto.MeanPrice) / 3;
                priceWriter.Write($"{moto.Make}, {moto.Year}, {moto.AveragePrice:f2}, {moto.MeanPrice:f2}, {combinedPrice:f2} {moto.MotorcycleCount}{Environment.NewLine}");
            }

            priceWriter.Dispose();
        }
    }
}