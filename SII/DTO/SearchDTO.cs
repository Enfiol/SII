using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SII
{
    public class SearchDTO
    {
        public string University { get; set; }
        public string Subject { get; set; }
        public string Language { get; set; }
        public string Author { get; set; }
        public double MinRating { get; set; }
        public double MaxRating { get; set; }
        public int MinPages { get; set; }
        public int MaxPages { get; set; }
        public int MinYear { get; set; }
        public int MaxYear { get; set; }
        public int MinThemes { get; set; }
        public int MaxThemes { get; set; }
        public List<int> StrictProperties { get; set; }
    }
}
