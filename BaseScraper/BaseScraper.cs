using BaseScraper.Calculations;
using BaseScraper.Config;
using BaseScraper.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using WebScraperV1.Data;

namespace BaseScraper;

public class Scraper
{
    public static async Task Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), StringsConstants.AppSettingsPath);
                                            
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath)
            .Build();

        var context = new MotoContext();
        ScraperSettings scraperSettings = new(configuration);

        //DatabaseSettings dbSettings = new(configuration);

        List<string> motorcycleMake = new();
        List<string> motorcycleCC = new();
        List<double> motorcyclePrice = new();
        List<string> motorcycleYear = new();
        List<string> announcementLink = new();

        using HttpClient client = new();

        try
        {
            string baseUrl = StringsConstants.MxBaseUrl;

            int maxPages = ScraperSettings.MaxPages;
            int doomCounter = 0;

            for (int i = 1; i <= maxPages; i++)
            {
                if (doomCounter > 2)
                {
                    i = ScraperSettings.MaxPages + 1;
                }

                string currentPage = baseUrl + i;

                HttpResponseMessage httpResponse = await client.GetAsync(currentPage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();

                    using StreamReader reader = new(contentStream, Encoding.GetEncoding(StringsConstants.Encoding));
                    string htmlContent = await reader.ReadToEndAsync();
                    HtmlDocument doc = new();
                    doc.LoadHtml(htmlContent);

                    var titleNodes = doc.DocumentNode.SelectNodes(StringsConstants.TitleNodes);
                    var priceNodes = doc.DocumentNode.SelectNodes(StringsConstants.PriceNodes);
                    var descriptionNodes = doc.DocumentNode.SelectNodes(StringsConstants.DescriptionNodes);
                    var linkNodes = doc.DocumentNode.SelectNodes(StringsConstants.LinkNodes);

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

                                string cc = StringsConstants.NotAvailable;

                                foreach (string cubicCent in titleTokens)
                                {
                                    Match ccMatch = Regex.Match(cubicCent, StringsConstants.CcPattern);
                                    if (ccMatch.Success)
                                    {
                                        string ccValue = ccMatch.Value;
                                        cc = ccValue;
                                        motorcycleCC.Add(cc);
                                    }
                                }
                                if (cc == StringsConstants.NotAvailable)
                                {
                                    motorcycleCC.Add(cc);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(StringsConstants.NoTitlesFound);
                    }

                    if (priceNodes != null)
                    {
                        foreach (var priceNode in priceNodes)
                        {
                            string priceInnerText = priceNode.InnerText;
                            string price = Regex.Replace(priceInnerText, StringsConstants.PriceIdentify, StringsConstants.PriceReplace);

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
                        doomCounter++;
                        Console.WriteLine(StringsConstants.NoPricesFound);
                    }

                    if (descriptionNodes != null)
                    {
                        foreach (var infoNode in descriptionNodes)
                        {
                            string infoText = infoNode.InnerText;
                            Match yearMatch = Regex.Match(infoText, StringsConstants.YearPattern);

                            if (yearMatch.Success)
                            {
                                string year = yearMatch.Value;
                                motorcycleYear.Add(year);
                            }
                            else
                            {
                                motorcycleYear.Add(StringsConstants.NotAvailable);
                            }
                        }
                    }
                    else
                    {
                        doomCounter++;
                        Console.WriteLine(StringsConstants.NoYearsFound);
                    }

                    if (linkNodes != null)
                    {
                        foreach (var href in linkNodes)
                        {
                            string link = href.GetAttributeValue(StringsConstants.HrefAttribute, StringsConstants.HrefDefault);

                            if (link.Length < 50)
                                continue;
                            else
                                announcementLink.Add(link);
                        }
                    }
                    else
                    {
                        Console.WriteLine(StringsConstants.NoLinksFound);
                    }
                }
                else
                {
                    Console.WriteLine(StringsConstants.FailedToRetreivePage);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
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

        using StreamWriter mxWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "MotocrossData.csv"));

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
                MeanPrice = MeanValues.MeanTrim(group.Select(m => m.Price), MeanValues.trimPercentage),
                DevPrice = MeanValues.Dev(group.Select(m => m.Price), MeanValues.deviationThreshold),
                MotorcycleCount = group.Count(),
            })
            .OrderBy(m => m.Make)
            .ThenBy(m => m.Year)
            .ThenBy(m => m.AveragePrice);

        using StreamWriter priceWriter = new(Path.Combine(ScraperSettings.OutputFolderPath, "AvgPriceMotocross.csv"));

        priceWriter.Write($"Make, Year, Average Price, Mean Price, StdDev Price, Combined Price, Count{Environment.NewLine}");

        foreach (var moto in averagePrices)
        {
            double combinedPrice = (moto.AveragePrice + moto.DevPrice + moto.MeanPrice) / 3;
            priceWriter.Write($"{moto.Make}, {moto.Year}, {moto.AveragePrice:f2}, {moto.MeanPrice:f2}, {moto.DevPrice:f2}, {combinedPrice:f2}, {moto.MotorcycleCount}{Environment.NewLine}");
        }

        priceWriter.Dispose();
    }
}