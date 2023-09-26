namespace BaseScraper
{
    public class Motorcycle
    {
		private string title;
		private string year;
		private string price;
        private string cc;

        public Motorcycle(string title, string year, string price)
        {
            Title = title;
            Year = year;
            Price = price;
        }

        public string Title
		{
			get { return title; }
			set { title = value; }
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

        public string CC
        {
            get { return cc; }
            set { cc = value; }
        }
    }
}
