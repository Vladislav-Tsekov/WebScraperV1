namespace BaseScraper.Models
{
    public class Motorcycle
    {
        private string make;
        private string cc;
        private string year;
        private double price;
        private string link;

        public Motorcycle(string make, string cc, string year, double price, string link)
        {
            Make = make;
            CC = cc;
            Year = year;
            Price = price;
            Link = link;
        }

        public string Make
        {
            get { return make; }
            set { make = value; }
        }

        public string CC
        {
            get { return cc; }
            set { cc = value; }
        }

        public string Year
        {
            get { return year; }
            set { year = value; }
        }

        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        public string Link
        {

            get { return link; }
            set { link = value; }
        }
    }
}
