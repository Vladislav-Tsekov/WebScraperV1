using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestScrapeV1._2_SC
{
    public class Scraper
    {
        private static readonly HttpClient client = new();
        private const string baseURL = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=u0mvgc&f1=";
        private const int maxPages = 21;

        public static async Task<List<Motorcycle>> ScrapeDataAsync()
        {
            List<string> motorcycleMake = new();
            List<string> motorcycleCC = new();
            List<double> motorcyclePrice = new();
            List<string> motorcycleYear = new();
            List<string> announcementLink = new();

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

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


                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve the web page.");
                        return null; // Handle the failure accordingly
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            // Create Motorcycle objects

            return motorcycles;
        }

        private static bool IsLinkValid(string link)
        {
            return link.Length >= 50 && !link.Contains("unwanted_keyword");
        }
    }
}
