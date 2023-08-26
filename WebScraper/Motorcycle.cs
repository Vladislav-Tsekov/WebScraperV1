namespace WebScraper
{
    public class Motorcycle
    {
        private string? make;
        private string? model;
        private int year;

        public Motorcycle(string make, string model, string cc, int year, int price)
        {
            Make = make;
            Model = model;
            CC = cc;
            Year = year;
            Price = price;
        }

        public string Make
        {
            get { return make; }
            private set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    Console.WriteLine(string.Format(Utilities.MakeIsNullOrEmpty));
                }
                make = value; 
            }
        }

        public string Model
        {
            get { return model; }
            private set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    Console.WriteLine(string.Format(Utilities.ModelIsNullOrEmpty));
                }
                model = value; 
            }
        }

        public string CC { get; private set; }

        public int Year
        {
            get { return year; }
            private set 
            {
                if (value < 2005)
                {
                    IsTooOld = true;
                }
                year = value; 
            }
        }

        public int Price { get; private set; }

        public bool IsTooOld { get; private set; }
    }
}
