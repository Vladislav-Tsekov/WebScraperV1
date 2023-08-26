namespace WebScraper
{
    public class Motorcycle
    {
        private string? make;
        private string? model;
        private int year;
        private bool isTooOld = false;

        public string Make
        {
            get { return make; }
            private set 
            {
                if (string.IsNullOrEmpty(value))
                {

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
                    
                }
                model = value; 
            }
        }

        public int CC { get; private set; }

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
