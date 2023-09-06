using System.Diagnostics;
using WebScraper.Models;
using WebScraper.Utilities;

class Engine
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello! You are currently using WebScraperV1, specifically tailored for Motocross and Enduro motorcycles. If you wish to gather data about a different vehicle type, please, exit this program.");

        while (true)
        {
            Console.WriteLine("Are you here to scavenge Motocross (M) or Enduro (E) motorcycles? (Options: M/E)");

            string path = Console.ReadLine();
            string subprocess = string.Empty;

            if (path.ToLower() == "m")
            {
                string pathToSubprocess = @"C:\Users\tseko\OneDrive\Documents\SoftUni\C# Personal Projects\WebScraperV1\MXScraper\bin\Debug\net6.0\MXScraper.exe";

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = pathToSubprocess,
                    UseShellExecute = false, // Set to false to redirect input, output, and error streams
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true // Set to true to hide the subprocess window
                };

                // Create and start the subprocess
                using (Process startMXScraper = new Process { StartInfo = startInfo })
                {
                    startMXScraper.Start();
                    startMXScraper.WaitForExit(); // Optionally wait for the subprocess to exit
                }

                break;
            }
            else if (path.ToLower() == "e")
            {
                EnduroScraper enduroScraper = new EnduroScraper();
                enduroScraper.RunEnduroScraper();
                break;
            }
            else
            {
                Console.WriteLine(string.Format(Messages.InvalidInput));
            }

            // Naked

            // Sport

            // Tourer
        }

    }
}