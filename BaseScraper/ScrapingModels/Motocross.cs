namespace BaseScraper.Models
{
    public class Motocross
    {
        public Motocross(string make, int cc, int year, decimal price, string link)
        {
            Make = make;
            CC = cc;
            Year = year;
            Price = price;
            Link = link;
        }

        public string Make { get; set; }

        public int CC { get; set; }

        public int Year { get; set; }

        public decimal Price { get; set; }

        public string Link { get; set; }
    }
}
