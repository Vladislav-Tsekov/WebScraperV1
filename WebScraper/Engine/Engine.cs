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

            if (path.ToLower() == "m")
            {
                MXScraper mxScraper = new MXScraper();
                mxScraper.RunMXScraper();
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
        }

    }
}