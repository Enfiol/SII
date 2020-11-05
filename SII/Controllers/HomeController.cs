using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SII.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("initdb")]
        public IActionResult InitDb()
        {
            Random rnd = new Random();
            List<Lection> lections = new List<Lection>();
            using (StreamReader sr = new StreamReader("attrib.txt"))
            {
                string input;
                int i = 1;
                while ((input = sr.ReadLine())!=null)
                {
                    var args = input.Split('_');
                    var status = args[5].Split('"', ':')[2];
                    var pages = sr.ReadLine().Split('_')[5].Split('"', ':')[2];
                    var editor = sr.ReadLine().Split('_')[5].Split('"', ':')[2];
                    var rating = sr.ReadLine().Split('_')[5].Split('"', ':')[2];
                    var readTime = sr.ReadLine().Split('_')[5].Split('"', ':')[2];
                    var year = sr.ReadLine().Split('_')[5].Split('"', ':')[2];
                    var maintheme = sr.ReadLine().Split('_')[5].Split('"', ':')[2];
                    var language = sr.ReadLine().Split('_')[5].Split('"', ':')[2];
                    
                    Lection lection = new Lection() { University = args[0], Author = args[1], Subject = args[2], Title = args[3], Language = language, Pages = int.Parse(pages), Rating = double.Parse(rating), Year = int.Parse(year) };
                    lection.Id = i;
                    lection.ThemesCount = rnd.Next(1, 4);
                    i++;
                    lections.Add(lection);
                }

            }
            return Ok(lections);
        }


       
    }
}
