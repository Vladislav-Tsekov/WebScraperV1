﻿using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;


// BASE SCRAPER - USED TO IDENTIFY AND AMEND ANY ERRORS, ESPECIALLY ONES CAUSED BY WRONG REG EX.

class Scraper
{
    static async Task Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        string baseURL = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=trjpk7&f1=1";

        // Insert a function that finds the "next" button - Web Crawler

        List<string> motorcycleTitle = new();
        List<string> motorcyclePrice = new();
        List<string> motorcycleYear = new();

        // Create an instance of HttpClient
        using HttpClient client = new();
        try
        {
            // Send an HTTP request to the URL
            HttpResponseMessage response = await client.GetAsync(baseURL);
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
                            Console.WriteLine("Empty Title");
                            continue;
                        }

                        Console.WriteLine($"Title: {title}");
                        motorcycleTitle.Add(title);
                    }
                }
                else
                {
                    Console.WriteLine("No motorcycle titles found on the page.");
                }

                // Extract the motorcycle prices using XPath

                if (priceNodes != null)
                {
                    foreach (var priceNode in priceNodes)
                    {
                        string priceInnerText = priceNode.InnerText;
                        string price = Regex.Replace(priceInnerText, @"[^\d]", ""); // SHOULD REMOVE ".ЛВ" AT THE END
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