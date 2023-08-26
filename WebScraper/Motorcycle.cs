namespace WebScraper
{
    public class Motorcycle
    {
        private string? make;
        private string? model;
        private int cc;
        private int year;
        private int price;

        public string Make
        {
            get { return make; }
            set { make = value; }
        }

        public string Model
        {
            get { return model; }
            set { model = value; }
        }

        public int CC
        {
            get { return cc; }
            set { cc = value; }
        }

        public int Year
        {
            get { return year; }
            set { year = value; }
        }

        public int Price
        {
            get { return price; }
            set { price = value; }
        }
    }
}
