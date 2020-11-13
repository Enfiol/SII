using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SII
{
    public class LectionSearchDTO
    {
        public string Title { get; set; }
        public string University { get; set; }
        public string Subject { get; set; }
        public string Language { get; set; }
        public string Author { get; set; }
        public double Rating { get; set; }
        public int Pages { get; set; }
        public int Year { get; set; }
        public int ThemesCount { get; set; }

        public string Strict { get; set; }
    }
}
