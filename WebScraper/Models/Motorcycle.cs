namespace WebScraper.Models
{
    public class Motorcycle
    {
        private string? make;
        private string? model;
        private string? cc;

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
                //if (string.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentNullException(string.Format(Utilities.MakeIsNullOrEmpty));
                //}
                make = value;
            }
        }

        public string Model
        {
            get { return model; }
            private set
            {
                //if (string.IsNullOrEmpty(value))
                //{
                //    throw new ArgumentNullException(string.Format(Utilities.ModelIsNullOrEmpty));
                //}
                model = value;
            }
        }

        public string CC
        {
            get => cc; private set
            {
                if (value.Length < 3 || value.Length > 3)
                {
                    cc = "N/A";
                    return;
                }
                cc = value;
            }
        }

        public int Year { get; private set; }

        public int Price { get; private set; }
    }
}
