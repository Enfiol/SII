using System;
using System.Collections.Generic;
using System.Text;

namespace ProximityMeasures
{
    public class Lection
    {
        public string Title { get; set; }
        public string University { get; set; } 
        public string Country { get; set; }
        public string Subject { get; set; }
        public string Language { get; set; } 
        public string Author { get; set; } 
        public double Rating { get; set; } 
        public int Pages { get; set; }
        public int Year { get; set; }
        public int ThemesCount { get; set; }

        //public int ReadTime { get; set; } //???
        //public string Maintheme { get; set; }
        //public bool Status { get; set; } //true - complete
    }
}
