﻿using BaseScraper.Calculations;
using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using static BaseScraper.Config.StringsConstants;
using static BaseScraper.Config.ScraperSettings;

namespace BaseScraper;

public class Scraper
{
    public static async Task Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), AppSettingsPath);

        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(appSettingsPath)
            .Build();

        ScraperSettings scraperSettings = new(configuration);

        List<string> motoMake = new();
        List<string> motoCc = new();
        List<double> motoPrice = new();
        List<string> motoYear = new();
        List<string> motoLink = new();

        using HttpClient client = new();

        try
        {
            string baseUrl = MxBaseUrl;

            int maxPages = MaxPages;
            int doomCounter = 0;

            for (int i = 1; i <= maxPages; i++)
            {
                if (doomCounter > 1)
                {
                    i = MaxPages + 1;
                }

                string currentPage = baseUrl + i;

                HttpResponseMessage httpResponse = await client.GetAsync(currentPage);

                if (httpResponse.IsSuccessStatusCode)
                {
                    Stream contentStream = await httpResponse.Content.ReadAsStreamAsync();

                    using StreamReader reader = new(contentStream, Encoding.GetEncoding(Encode));
                    string htmlContent = await reader.ReadToEndAsync();
                    HtmlDocument doc = new();
                    doc.LoadHtml(htmlContent);

                    var titleNodes = doc.DocumentNode.SelectNodes(TitleNodes);
                    var priceNodes = doc.DocumentNode.SelectNodes(PriceNodes);
                    var descriptionNodes = doc.DocumentNode.SelectNodes(DescriptionNodes);
                    var linkNodes = doc.DocumentNode.SelectNodes(LinkNodes);

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
                                motoMake.Add(make);

                                string cc = NotAvailable;

                                foreach (string cubicCent in titleTokens)
                                {
                                    
                                    Match ccMatch = Regex.Match(cubicCent, CcPattern);
                                    bool isMatched = false;

                                    if (ccMatch.Success && isMatched == false)
                                    {
                                        string ccValue = ccMatch.Value;
                                        cc = ccValue;
                                        motoCc.Add(cc);
                                        isMatched = true;
                                    }

                                    if (isMatched)
                                    {
                                        break;
                                    }
                                }
                                if (cc == NotAvailable)
                                {
                                    motoCc.Add(cc);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(NoTitlesFound);
                    }

                    if (priceNodes != null)
                    {
                        foreach (var priceNode in priceNodes)
                        {
                            string priceInnerText = priceNode.InnerText;
                            string price = Regex.Replace(priceInnerText, PriceIdentify, PriceReplace);

                            if (double.TryParse(price, out double priceValue))
                            {
                                motoPrice.Add(priceValue);
                            }
                            else
                            {
                                motoPrice.Add(0);
                            }
                        }
                    }
                    else
                    {
                        doomCounter++;
                        Console.WriteLine(NoPricesFound);
                    }

                    if (descriptionNodes != null)
                    {
                        foreach (var infoNode in descriptionNodes)
                        {
                            string infoText = infoNode.InnerText;
                            Match yearMatch = Regex.Match(infoText, YearPattern);

                            if (yearMatch.Success)
                            {
                                string year = yearMatch.Value;
                                motoYear.Add(year);    
                            }
                            else
                            {
                                motoYear.Add(YearIsNull);
                            }
                        }
                    }
                    else
                    {
                        doomCounter++;
                        Console.WriteLine(NoYearsFound);
                    }

                    if (linkNodes != null)
                    {
                        foreach (var href in linkNodes)
                        {
                            string link = href.GetAttributeValue(HrefAttribute, HrefDefault);

                            if (link.Length < 50)
                                continue;
                            else
                            {
                                string modifiedLink = link[2..63];
                                motoLink.Add(modifiedLink);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(NoLinksFound);
                    }
                }
                else
                {
                    Console.WriteLine(FailedToRetreivePage);
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        HashSet<Motorcycle> motorcycles = new();

        for (int i = 0; i < motoMake.Count; i++)
        {
            Motorcycle motorcycle = new(motoMake[i], motoCc[i], motoYear[i], motoPrice[i], motoLink[i]);
            motorcycles.Add(motorcycle);
        }

        List<Motorcycle> scrapedMoto = motorcycles
                            .Where(m => m.Price > 3000)
                            .OrderBy(m => m.Make)
                            .ThenBy(m => m.Year)
                            .ThenBy(m => m.Price)
                            .ToList();

        DataExport dataExport = new();
        MotoContext context = new();

        List<string> distinctMakes = motoMake.Distinct().OrderBy(x => x).ToList();
        List<int> distinctYears = motoYear.Select(s => int.Parse(s)).Distinct().OrderBy(x => x).ToList();

        await dataExport.PopulateMakesTable(distinctMakes, context);
        await dataExport.PopulateYearsTable(distinctYears, context);
        await dataExport.AddMotorcycleEntries(scrapedMoto, context);
        await dataExport.AddMarketPrices(scrapedMoto, context);
        await dataExport.TransferSoldEntries(context);

        DataAnalysis dataAnalysis = new();
        SaleReport saleReport = new();
        MarketOverview marketOverview = new();

        await dataAnalysis.MarketOverviewReport(context, marketOverview);
        await dataAnalysis.SoldMotorcyclesReport(context, saleReport);

        //TODO - FOLLOW THE PREVIOUS TWO METHODS' PATTERN IF THE METHOD GROWS LARGER
        StreamWriter marketOutliers = new(Path.Combine(OutputFolderPath, "MarketOutliers.csv"));
        marketOutliers.WriteLine($"{DateTime.Now:d}");
        await dataAnalysis.UnusualValuesReport(context, marketOutliers);
        marketOutliers.Dispose();
    }
}