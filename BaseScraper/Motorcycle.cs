namespace BaseScraper
{
    public class Motorcycle
    {
		private string title;
        private string cc;
		private string year;
		private string price;

        public Motorcycle(string title, string cc, string year, string price)
        {
            Title = title;
            Year = year;
            Price = price;
            CC = cc;
        }

        public string Title
		{
			get { return title; }
			set { title = value; }
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
