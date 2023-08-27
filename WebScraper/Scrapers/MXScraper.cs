using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace WebScraper.Models
{
    public class MXScraper
    {
        public void RunMXScraper()
        {
            static async Task Main(string[] args)
            {
                // Register 'windows-1251' encoding provider in order to read Cyrillic
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                List<string[]> motorcycleTitles = new();
                List<int> motorcycleYears = new();
                List<int> motorcyclePrices = new();

                using HttpClient client = new();
                try
                {
                    // BASE URL IS USED BECAUSE THE URL STRUCTURE IS SIMILAR AND ONLY THE LAST SYMBOL (COUNTER) CHANGES
                    string motocrossBaseURL = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=tk66ao&f1=";

                    // MAX NUMBER OF PAGES TO SCRAPE
                    int maxPages = 10; // ADJUST THIS IF NEEDED

                    for (int i = 0; i < maxPages; i++)
                    {
                        string currentPageURL = "https://www.mobile.bg/pcgi/mobile.cgi?act=3&slink=tk66ao&f1=" + i;

                        HttpResponseMessage response = await client.GetAsync(currentPageURL);

                        if (response.IsSuccessStatusCode)
                        {
                            Stream contentStream = await response.Content.ReadAsStreamAsync();

                            using StreamReader reader = new(contentStream, Encoding.GetEncoding("windows-1251"));

                            string htmlContent = await reader.ReadToEndAsync();

                            HtmlDocument doc = new();
                            doc.LoadHtml(htmlContent);

                            var titleNodes = doc.DocumentNode.SelectNodes("//a[@class='mmm']");

                            if (titleNodes != null)
                            {
                                foreach (var titleNode in titleNodes)
                                {
                                    string title = titleNode.InnerText;

                                    string makePattern = @"^(.*?)(?:\s+|$)";
                                    string modelPattern = @"(?:\s+|^)(.*?)(?:\s+\d+|$)";
                                    string ccPattern = @"(?:\s+|^)(\d{3})(?:\s+|$)";

                                    Match makeMatch = Regex.Match(title, makePattern);
                                    Match modelMatch = Regex.Match(title, modelPattern);
                                    Match ccMatch = Regex.Match(title, ccPattern);

                                    string make = makeMatch.Success ? makeMatch.Groups[1].Value.Trim() : "N/A";
                                    string model = modelMatch.Success ? modelMatch.Groups[1].Value.Trim() : "N/A";
                                    string cc = ccMatch.Success ? ccMatch.Groups[1].Value.Trim() : "N/A";

                                    if (string.IsNullOrEmpty(make))
                                    {
                                        make = "N/A";
                                        //continue;
                                    }

                                    model = model.Replace(make, "").Trim();
                                    int firstSpaceIndex = model.IndexOf(" ");
                                    if (firstSpaceIndex >= 0)
                                    {
                                        model = model.Substring(0, firstSpaceIndex);
                                    }

                                    if (string.IsNullOrEmpty(model))
                                    {
                                        model = "N/A";
                                        //continue;
                                    }

                                    string[] motoTitle = { make, model, cc };

                                    motorcycleTitles.Add(motoTitle);
                                }
                            }
                            else
                            {
                                Console.WriteLine(string.Format(Utilities.Messages.NoTitleFound));
                            }

                            var priceNodes = doc.DocumentNode.SelectNodes("//span[@class='price']");

                            if (priceNodes != null)
                            {
                                foreach (var priceNode in priceNodes)
                                {
                                    string priceInnerText = priceNode.InnerText;
                                    string price = Regex.Replace(priceInnerText, @"[^\d]", "");
                                    int convertedPrice = int.Parse(price);
                                    motorcyclePrices.Add(convertedPrice);
                                    Console.WriteLine(convertedPrice);
                                }
                            }
                            else
                            {
                                Console.WriteLine(string.Format(Utilities.Messages.NoPriceFound));
                            }

                            var infoNodes = doc.DocumentNode.SelectNodes("//td[(contains(@colspan,'3') or contains(@colspan,'4')) and contains(@style,'padding-left:')]");

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
                                        int convertedYear = int.Parse(year);
                                        motorcycleYears.Add(convertedYear);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine(string.Format(Utilities.Messages.NoYearFound));
                            }
                        }
                        else
                        {
                            Console.WriteLine(string.Format(Utilities.Messages.WebpageFailed));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }

                List<Motorcycle> allMotorcycles = new();

                for (int i = 0; i < motorcycleTitles.Count; i++)
                {
                    //Console.WriteLine($"{motorcycleTitles[i][0]}, {motorcycleTitles[i][1]}, {motorcycleTitles[i][2]}, {motorcycleYears[i]}, {motorcyclePrices[i]}");
                    var currentMotorcycle = new Motorcycle(motorcycleTitles[i][0], motorcycleTitles[i][1], motorcycleTitles[i][2], motorcycleYears[i], motorcyclePrices[i]);

                    allMotorcycles.Add(currentMotorcycle);
                }

                var sortedMotorcycles = allMotorcycles.OrderByDescending(m => m.Make).ThenBy(m => m.Year).ThenBy(m => m.Price);

                //EXPORT MOTOCROSS DATA TO TEXT FILE
                using StreamWriter mxWriter = new(@"../../../Motocross.txt");

                foreach (var motorcycle in sortedMotorcycles)
                {
                    mxWriter.Write($"{motorcycle.Make}, {motorcycle.Model}, {motorcycle.CC}, {motorcycle.Year}, {motorcycle.Price}{Environment.NewLine}");
                }

                Console.WriteLine($"Scraping has ended, {motorcycleTitles.Count} motorcycles were scraped successfully!");
            }
        }
    }
}
