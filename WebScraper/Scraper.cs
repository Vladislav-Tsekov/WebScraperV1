using System;
using System.Net.Http;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        // Register the 'windows-1251' encoding provider
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        // URL of the website to scrape
        string url = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=thgxfy&f1=1";

        // Create an instance of HttpClient to send HTTP requests
        using (HttpClient client = new HttpClient())
        {
            try
            {
                // Send an HTTP GET request to the URL
                HttpResponseMessage response = await client.GetAsync(url);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Read the content of the response as a stream
                    Stream contentStream = await response.Content.ReadAsStreamAsync();

                    // Read the stream with the correct encoding (windows-1251 for Cyrillic)
                    using (StreamReader reader = new StreamReader(contentStream, Encoding.GetEncoding("windows-1251")))
                    {
                        string htmlContent = await reader.ReadToEndAsync();

                        // Parse the HTML content using HtmlAgilityPack
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(htmlContent);

                        // Extract the motorcycle names using XPath
                        var nameNodes = doc.DocumentNode.SelectNodes("//a[@class='mmm']");

                        if (nameNodes != null)
                        {
                            foreach (var nameNode in nameNodes)
                            {
                                string name = nameNode.InnerText;
                                if (string.IsNullOrEmpty(name))
                                {
                                    continue;
                                }
                                Console.WriteLine($"Name: {name}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No motorcycle names found on the page.");
                        }

                        // Extract the motorcycle prices using XPath
                        var priceNodes = doc.DocumentNode.SelectNodes("//span[@class='price']");

                        if (priceNodes != null)
                        {
                            foreach (var priceNode in priceNodes)
                            {
                                string price = priceNode.InnerText;
                                Console.WriteLine($"Price: {price}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("No motorcycle prices found on the page.");
                        }

                        // Extract the motorcycle year using regular expressions
                        var infoNodes = doc.DocumentNode.SelectNodes("//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:4px')]");


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
                                    Console.WriteLine($"Year: {year}");
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("No motorcycle information found on the page.");
                        }
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
        }
    }
}



