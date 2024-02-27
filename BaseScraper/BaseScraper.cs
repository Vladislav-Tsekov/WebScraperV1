using BaseScraper.Config;
using BaseScraper.Data;
using BaseScraper.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using static BaseScraper.Config.ScraperSettings;
using static BaseScraper.Config.StringsConstants;

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

        List<string> makes = new();
        List<int> displacements = new();
        List<decimal> prices = new();
        List<int> years = new();
        List<string> links = new();

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

                    HtmlNodeCollection titleNodes = doc.DocumentNode.SelectNodes(TitleNodes);
                    HtmlNodeCollection priceNodes = doc.DocumentNode.SelectNodes(PriceNodes);
                    HtmlNodeCollection descriptionNodes = doc.DocumentNode.SelectNodes(DescriptionNodes);
                    HtmlNodeCollection linkNodes = doc.DocumentNode.SelectNodes(LinkNodes);

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
                                makes.Add(make);

                                int cc = CcNotAvailable;

                                foreach (string cubicCent in titleTokens)
                                {
                                    Match ccMatch = Regex.Match(cubicCent, CcPattern);

                                    if (ccMatch.Success)
                                    {
                                        string ccValue = ccMatch.Value;
                                        cc = int.Parse(ccValue);
                                        displacements.Add(cc);
                                        break;
                                    }
                                }

                                if (cc == CcNotAvailable)
                                {
                                    displacements.Add(cc);
                                }
                            }
                        }
                    }
                    else
                    {
                        doomCounter++;
                        Console.WriteLine(NoTitlesFound);
                    }

                    if (priceNodes != null)
                    {
                        foreach (var priceNode in priceNodes)
                        {
                            string priceInnerText = priceNode.InnerText;
                            string priceEdit = Regex.Replace(priceInnerText, PriceIdentify, PriceReplace);

                            if (decimal.TryParse(priceEdit, out decimal price))
                            {
                                prices.Add(price);
                            }
                            else
                            {
                                prices.Add(0);
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
                                int year = int.Parse(yearMatch.Value);
                                years.Add(year);    
                            }
                            else
                            {
                                years.Add(YearIsNull);
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

                            //CHECK LINK LENGTH IN CASE OF ERRORS
                            string modifiedLink = link[2..40];
                            links.Add(modifiedLink);
                        }
                    }
                    else
                    {
                        doomCounter++;
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

        HashSet<Motocross> motorcycles = new();

        for (int i = 0; i < makes.Count; i++)
        {
            Motocross currentMoto = new(makes[i], displacements[i], years[i], prices[i], links[i]);
            motorcycles.Add(currentMoto);
        }

        List<Motocross> scrapedMoto = motorcycles
                            .Where(m => m.Price > 3000 && m.Year >= 2006 && m.Year <= 2030)
                            .OrderBy(m => m.Make)
                            .ThenBy(m => m.Year)
                            .ThenBy(m => m.Price)
                            .ToList();


        List<string> distinctMakes = makes.Distinct().OrderBy(m => m).ToList();
        List<int> distinctYears = years.Distinct().OrderBy(y => y).ToList();

        MotoContext context = new();

        await DataExport.UpdateMakesTable(distinctMakes, context);
        await DataExport.UpdateYearsTable(distinctYears, context);
        await DataExport.AddMotorcycleEntries(scrapedMoto, context);
        await DataExport.AddMarketPrices(scrapedMoto, context);
        await DataExport.TransferSoldEntries(context);

        await DataAnalysis.MarketOverviewReport(context);
        await DataAnalysis.SoldMotorcyclesReport(context);
        await DataAnalysis.UnusualValuesReport(context);

        await context.DisposeAsync();
    }
}