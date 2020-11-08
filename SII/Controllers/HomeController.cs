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
        private readonly ApplicationContext _db;

        public HomeController(ILogger<HomeController> logger, ApplicationContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
          
            return View();
        }

        [HttpGet("usermarks/{id}")]
        public IActionResult GetUserMarks(int id)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            List<UserMark> list = _db.UserMarks.Where(um => um.UserId == user.Id).ToList();
            return Ok(list);
        }

        [HttpGet("initdb")]
        public IActionResult InitDb()
        {
            Random rnd = new Random();
            List<Lection> lections = new List<Lection>();
            int i = 1;
            using (StreamReader sr = new StreamReader("attrib.txt"))
            {
                //init lections
                string input;
                
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
            foreach(Lection l in lections)
            {
                if(_db.Lections.FirstOrDefault(lect => lect.Id == l.Id)==null)
                {
                    _db.Lections.Add(l);
                }
            }

            //init users
            for(int j = 1; j<=5; j++)
            {
                User user = new User() { Id = j, Name = "User" + j };
                if (_db.Users.FirstOrDefault(u => u.Id == user.Id) == null)
                {
                    _db.Users.Add(user);
                }
                
            }

            //init usermarks
            List<UserMark> userMarks = new List<UserMark>();
            userMarks.Add(new UserMark() { Id = 1, UserId = 1, LectionId = 1, Mark = 4});
            userMarks.Add(new UserMark() { Id = 2, UserId = 1, LectionId = 2, Mark = 5 });
            userMarks.Add(new UserMark() { Id = 3, UserId = 1, LectionId = 3, Mark = 4 });
            userMarks.Add(new UserMark() { Id = 4, UserId = 1, LectionId = 5, Mark = 4 });
            userMarks.Add(new UserMark() { Id = 22, UserId = 1, LectionId = 6, Mark = 4 });
            userMarks.Add(new UserMark() { Id = 24, UserId = 1, LectionId = 9, Mark = 4 });

            userMarks.Add(new UserMark() { Id = 5, UserId = 2, LectionId = 2, Mark = 5 });
            userMarks.Add(new UserMark() { Id = 6, UserId = 2, LectionId = 3, Mark = 5 });
            userMarks.Add(new UserMark() { Id = 7, UserId = 2, LectionId = 4, Mark = 5 });
            userMarks.Add(new UserMark() { Id = 8, UserId = 2, LectionId = 5, Mark = 5 });

            userMarks.Add(new UserMark() { Id = 9, UserId = 3, LectionId = 2, Mark = 2 });
            userMarks.Add(new UserMark() { Id = 10, UserId = 3, LectionId = 3, Mark = 3 });
            userMarks.Add(new UserMark() { Id = 11, UserId = 3, LectionId = 4, Mark = 2 });
            userMarks.Add(new UserMark() { Id = 12, UserId = 3, LectionId = 5, Mark = 1 });

            userMarks.Add(new UserMark() { Id = 13, UserId = 4, LectionId = 1, Mark = 5 });
            userMarks.Add(new UserMark() { Id = 14, UserId = 4, LectionId = 2, Mark = 3 });
            userMarks.Add(new UserMark() { Id = 15, UserId = 4, LectionId = 3, Mark = 2 });
            userMarks.Add(new UserMark() { Id = 16, UserId = 4, LectionId = 4, Mark = 5 });
            userMarks.Add(new UserMark() { Id = 17, UserId = 4, LectionId = 5, Mark = 3 });

            userMarks.Add(new UserMark() { Id = 18, UserId = 5, LectionId = 1, Mark = 4 });
            userMarks.Add(new UserMark() { Id = 19, UserId = 5, LectionId = 2, Mark = 5 });
            userMarks.Add(new UserMark() { Id = 20, UserId = 5, LectionId = 3, Mark = 4 });
            userMarks.Add(new UserMark() { Id = 21, UserId = 5, LectionId = 5, Mark = 4 });
            userMarks.Add(new UserMark() { Id = 26, UserId = 5, LectionId = 6, Mark = 3 });
            userMarks.Add(new UserMark() { Id = 27, UserId = 5, LectionId = 7, Mark = 4 });
            userMarks.Add(new UserMark() { Id = 28, UserId = 5, LectionId = 9, Mark = 4 });
            userMarks.Add(new UserMark() { Id = 29, UserId = 5, LectionId = 10, Mark = 5 });




            foreach (UserMark um in userMarks)
            {
                if (_db.UserMarks.FirstOrDefault(umark => umark.Id == um.Id) == null)
                {
                    _db.UserMarks.Add(um);
                }
            }

            _db.SaveChanges();

            return Ok(lections);
        }


       
    }
}
