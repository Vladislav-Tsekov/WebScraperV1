namespace BaseScraper
{
    public class Motorcycle
    {
		private string make;
        private string cc;
		private string year;
		private string price;

        public Motorcycle(string make, string cc, string year, string price)
        {
            Make = make;
            CC = cc;
            Year = year;
            Price = price;
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

        public string Price
        {
            get { return price; }
            set { price = value; }
        }


    }
}
