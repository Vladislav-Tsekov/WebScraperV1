﻿using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Data.Models;
using BaseScraper.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.RegularExpressions;

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

        ScraperSettings scraperSettings = new(configuration);

        List<string> motoMake = new();
        List<string> motoCc = new();
        List<double> motoPrice = new();
        List<string> motoYear = new();
        List<string> motoLink = new();

        using HttpClient client = new();

        try
        {
            string baseUrl = StringsConstants.MxBaseUrl;

            int maxPages = ScraperSettings.MaxPages;
            int doomCounter = 0;

            for (int i = 1; i <= maxPages; i++)
            {
                if (doomCounter > 1)
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
                                motoMake.Add(make);

                                string cc = StringsConstants.NotAvailable;

                                foreach (string cubicCent in titleTokens)
                                {
                                    Match ccMatch = Regex.Match(cubicCent, StringsConstants.CcPattern);
                                    if (ccMatch.Success)
                                    {
                                        string ccValue = ccMatch.Value;
                                        cc = ccValue;
                                        motoCc.Add(cc);
                                    }
                                }
                                if (cc == StringsConstants.NotAvailable)
                                {
                                    motoCc.Add(cc);
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
                                motoYear.Add(year);
                            }
                            else
                            {
                                motoYear.Add(StringsConstants.YearIsNull);
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
                                motoLink.Add(link);
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

        HashSet<Motorcycle> motorcycles = new();

        for (int i = 0; i < motoMake.Count; i++)
        {
            Motorcycle motorcycle = new(motoMake[i], motoCc[i], motoYear[i], motoPrice[i], motoLink[i]);
            motorcycles.Add(motorcycle);
        }

        List<Motorcycle> filteredMoto = motorcycles
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
        await dataExport.AddMotorcycleEntries(filteredMoto, context);
        await dataExport.CalculateMarketPrices(filteredMoto, context);

        DataAnalysis dataAnalysis = new();

        await dataAnalysis.TotalMotorcyclesCountByMake(context);
        await dataAnalysis.TotalMotorcyclesCountByYear(context);
    }
}