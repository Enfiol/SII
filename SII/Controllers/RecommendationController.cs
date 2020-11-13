using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SII.Controllers
{
    [Route("recommendation")]
    public class RecommendationController : Controller
    {
        private readonly ILogger<RecommendationController> _logger;
        private readonly ApplicationContext _db;

        public RecommendationController(ILogger<RecommendationController> logger, ApplicationContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<User> users = _db.Users.ToList();
            return View(users);
        }

        [HttpGet("collaboration/{id}")]
        public IActionResult Collaboration(int id)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            List<User> users = _db.Users.Where(u => u.Id != id).ToList();

            List<UserCoeff> userCoeffs = new List<UserCoeff>();
            foreach (User u in users)
            {
                List<UserMark> usermarks1 = _db.UserMarks.Where(um => um.UserId == user.Id).ToList();
                List<UserMark> usermarks2 = _db.UserMarks.Where(um => um.UserId == u.Id).ToList();

                int lectionsNumber = _db.Lections.Count();

                List<double> list1 = new double[lectionsNumber].ToList();
                List<double> list2 = new double[lectionsNumber].ToList();

                foreach (UserMark um in usermarks1)
                {
                    list1[um.LectionId] = um.Mark;
                }
                foreach (UserMark um in usermarks2)
                {
                    list2[um.LectionId] = um.Mark;
                }

                while (true)
                {

                    int index1 = list1.IndexOf(0);
                    if (index1 != -1)
                    {
                        list1.RemoveAt(index1);
                        list2.RemoveAt(index1);
                    }
                    int index2 = list2.IndexOf(0);
                    if (index2 != -1)
                    {
                        list1.RemoveAt(index2);
                        list2.RemoveAt(index2);
                    }
                    if (index1 == -1 && index2 == -1)
                    {
                        break;
                    }
                }
                double coeff = CollaborationFilter.Correlation(list1.ToArray(), list2.ToArray());
                userCoeffs.Add(new UserCoeff() { Coeff = coeff, UserId = u.Id });
            }


            userCoeffs = userCoeffs.Take(5).OrderByDescending(u => u.Coeff).ToList();

            List<double> reccomend = new double[_db.Lections.Count()].ToList();

            foreach (UserCoeff uc in userCoeffs)
            {
                var userMarks = _db.UserMarks.Where(um => um.UserId == uc.UserId);
                foreach (var mark in userMarks)
                {
                    reccomend[mark.LectionId - 1] += mark.Mark * uc.Coeff;
                }
            }

            List<LectionCoeff> lc = new List<LectionCoeff>();
            List<int> alreadyMarked = _db.UserMarks.Where(um => um.UserId == id).Select(um => um.LectionId).ToList();
            List<int> dontShow = _db.UserLections.Where(um => um.UserId == id).Select(um => um.LectionId).ToList();
            for (int i = 0; i < reccomend.Count; i++)
            {
                if (reccomend[i] > 0 && alreadyMarked.IndexOf(i + 1) == -1 && dontShow.IndexOf(i+1) == -1)
                {
                    lc.Add(new LectionCoeff() { Id = i + 1, Coeff = reccomend[i], UserId = id});
                }
            }

            return View(lc.OrderByDescending(o => o.Coeff).ToList());
        }

        [HttpGet("dontshowagain/{userId}")]
        public IActionResult DontShowAgain(int userId, int lectionId)
        {
            UserLection ul = new UserLection() { LectionId = lectionId, UserId = userId };
            _db.UserLections.Add(ul);
            _db.SaveChanges();
            return RedirectToAction("Collaboration", new { id = userId });
        }
        [HttpGet("dontshowagaincontent/{userId}")]
        public IActionResult DontShowAgainContent(int userId, int lectionId)
        {
            UserLection ul = new UserLection() { LectionId = lectionId, UserId = userId };
            _db.UserLections.Add(ul);
            _db.SaveChanges();
            return RedirectToAction("Content", new { id = userId });
        }

        [HttpGet("correlation/{id}")]
        public IActionResult Correlation(int id)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            List<User> users = _db.Users.Where(u => u.Id != id).ToList();

            List<UserCoeff> userCoeffs = new List<UserCoeff>();
            foreach (User u in users)
            {
                List<UserMark> usermarks1 = _db.UserMarks.Where(um => um.UserId == user.Id).ToList();
                List<UserMark> usermarks2 = _db.UserMarks.Where(um => um.UserId == u.Id).ToList();

                int lectionsNumber = _db.Lections.Count();

                List<double> list1 = new double[lectionsNumber].ToList();
                List<double> list2 = new double[lectionsNumber].ToList();

                foreach (UserMark um in usermarks1)
                {
                    list1[um.LectionId] = um.Mark;
                }
                foreach (UserMark um in usermarks2)
                {
                    list2[um.LectionId] = um.Mark;
                }

                while (true)
                {

                    int index1 = list1.IndexOf(0);
                    if (index1 != -1)
                    {
                        list1.RemoveAt(index1);
                        list2.RemoveAt(index1);
                    }
                    int index2 = list2.IndexOf(0);
                    if (index2 != -1)
                    {
                        list1.RemoveAt(index2);
                        list2.RemoveAt(index2);
                    }
                    if (index1 == -1 && index2 == -1)
                    {
                        break;
                    }
                }
                double coeff = CollaborationFilter.Correlation(list1.ToArray(), list2.ToArray());
                userCoeffs.Add(new UserCoeff() { Coeff = coeff, UserId = u.Id });
            }

            return View(userCoeffs.OrderByDescending(c => c.Coeff).ToList());
        }


        [HttpGet("lections")]
        public IActionResult Lections()
        {
            var lections = _db.Lections.ToList();
            return View(lections);
        }

        [HttpGet("similarlections/{id}")]
        public IActionResult SimilarLections(int id)
        {
            Lection lection = _db.Lections.FirstOrDefault(l => l.Id == id);
            if(lection == null)
            {
                return NotFound();
            }

            List<double> coeffsList = new double[_db.Lections.Count()].ToList();
            List<Lection> lections = _db.Lections.Where(l => l.Id != id).ToList();

            foreach(Lection l in lections)
            {
                coeffsList[l.Id - 1] = Measures.CorrelationDistance(lection, l)*Measures.EqualValues(lection,l);
            }


            List<LectionCoeff> lc = new List<LectionCoeff>();
            for(int i = 0; i<coeffsList.Count; i++)
            {
                if(coeffsList[i]>0)
                {
                    lc.Add(new LectionCoeff { Id = i + 1, Coeff = coeffsList[i] });
                }
            }

            return View(lc.OrderByDescending(c => c.Coeff).ToList());
        }


        
        [HttpPost("similarlections")]
        public IActionResult SimilarLections(CheckboxDTO check)
        {
            if (check.AreChecked == null)
            {
                return View(new List<LectionCoeff>());
            }
            List<Lection> lections = new List<Lection>();
            foreach(int i in check.AreChecked)
            {
                lections.Add(_db.Lections.FirstOrDefault(l => l.Id == i));
            }

            if (lections.Count == 0)
            {
                return View(new List<LectionCoeff>());
            }

            
            List<Lection> allLections = _db.Lections.ToList();
            int dec = 1;
            foreach (int i in check.AreChecked)
            {
                allLections.RemoveAt(i - dec);
                dec++;
            }
            if (allLections.Count == 0)
            {
                return View(new List<LectionCoeff>());
            }

            double[,] coeffsMatrix = new double[_db.Lections.Count(), check.AreChecked.Count];
            int index = 0;
            foreach (Lection l in lections)
            {
                foreach (Lection al in allLections)
                {
                    coeffsMatrix[al.Id - 1,index] = Measures.CorrelationDistance(l, al) * Measures.EqualValues(l, al);
                }
                index++;
            }


            List<double> coeffsList = new double[allLections.Count].ToList();
            for(int j = 0; j < allLections.Count; j++)
            {
                for(int i = 0; i < lections.Count; i++)
                {
                    coeffsList[j] += coeffsMatrix[j, i];
                }
            }

            List<LectionCoeff> lc = new List<LectionCoeff>();
            for (int i = 0; i < coeffsList.Count; i++)
            {
                if (coeffsList[i] > 0)
                {
                    lc.Add(new LectionCoeff { Id = i + 1, Coeff = coeffsList[i] });
                }
            }

            

            return View(lc.OrderByDescending(c => c.Coeff).ToList());
        }

        [HttpGet("Content/{id}")]
        public IActionResult Content(int id)
        {


            List<UserMark> userMarks = _db.UserMarks.Where(um => um.UserId == id).ToList();
            List<Lection> lections = new List<Lection>();
            foreach (UserMark um in userMarks)
            {
                lections.Add(_db.Lections.FirstOrDefault(l => l.Id == um.LectionId));
            }

            if (lections.Count == 0)
            {
                return View(new List<LectionCoeff>());
            }


            double[,] coeffsMatrix = new double[_db.Lections.Count(), lections.Count];
            List<Lection> allLections = _db.Lections.ToList();

            allLections = allLections.Except(lections).ToList();

            if (allLections.Count == 0)
            {
                return View(new List<LectionCoeff>());
            }

            int index = 0;
            foreach (Lection l in lections)
            {
                foreach (Lection al in allLections)
                {
                    coeffsMatrix[al.Id - 1, index] = Measures.CorrelationDistance(l, al) * Measures.EqualValues(l, al);
                }
                index++;
            }


            List<double> coeffsList = new double[allLections.Count].ToList();
            for (int j = 0; j < allLections.Count; j++)
            {
                for (int i = 0; i < lections.Count; i++)
                {
                    coeffsList[j] += coeffsMatrix[j, i];
                }
            }

            List<int> dontshow = _db.UserLections.Where(ul => ul.UserId == id).Select(ul => ul.LectionId).ToList();

            List<LectionCoeff> lc = new List<LectionCoeff>();
            for (int i = 0; i < coeffsList.Count; i++)
            {
                if (coeffsList[i] > 0 && dontshow.IndexOf(i + 1) == -1)
                {
                    lc.Add(new LectionCoeff { Id = i + 1, Coeff = coeffsList[i], UserId = id });
                }
            }



            return View(lc.OrderByDescending(c => c.Coeff).ToList());
        }

        [HttpGet("Search")]
        public IActionResult Search(SearchDTO searchDTO)
        {
            if(searchDTO.Author==null)
            {
                return View(new List<LectionSearchDTO>());
            }

            var strictLections = _db.Lections.Where(l => l.Language == searchDTO.Language).ToList();
            strictLections = strictLections.Where(l => l.Subject == searchDTO.Subject).ToList();
            strictLections = strictLections.Where(l => l.Author == searchDTO.Author).ToList();
            strictLections = strictLections.Where(l => l.University == searchDTO.University).ToList();
            strictLections = strictLections.Where(l => l.Pages >= searchDTO.MinPages && l.Pages <= searchDTO.MaxPages).ToList();
            strictLections = strictLections.Where(l => l.Rating >= searchDTO.MinRating && l.Rating <= searchDTO.MaxRating).ToList();
            strictLections = strictLections.Where(l => l.ThemesCount >= searchDTO.MinThemes && l.ThemesCount <= searchDTO.MaxThemes).ToList();
            strictLections = strictLections.Where(l => l.Year >= searchDTO.MinYear && l.Year <= searchDTO.MaxYear).ToList();


            List<LectionSearchDTO> result = new List<LectionSearchDTO>();
            foreach(Lection l in strictLections)
            {
                result.Add(new LectionSearchDTO()
                {
                    Title = l.Title,
                    Author = l.Author,
                    Language = l.Language,
                    Pages = l.Pages,
                    Rating = l.Rating,
                    Subject = l.Subject,
                    ThemesCount = l.ThemesCount,
                    University = l.University,
                    Year = l.Year,
                    Strict = "Strict"
                });
            }

            if(strictLections.Count() == 0)
            {
                strictLections.Add(new Lection
                {
                    Author = searchDTO.Author,
                    Language = searchDTO.Language,
                    Pages = (searchDTO.MinPages + searchDTO.MaxPages) / 2,
                    Subject = searchDTO.Subject,
                    Rating = (searchDTO.MinRating + searchDTO.MaxRating) / 2,
                    ThemesCount = (searchDTO.MinThemes + searchDTO.MaxThemes) / 2,
                    University = searchDTO.University,
                    Year = (searchDTO.MinYear + searchDTO.MaxYear) / 2
                });
            }
            var notstrictLections = _db.Lections.ToList();
            if (searchDTO.StrictProperties.IndexOf(1) != -1)
            {
                notstrictLections = notstrictLections.Where(l => l.University == searchDTO.University).ToList();
            }
            if (searchDTO.StrictProperties.IndexOf(2) != -1)
            {
                notstrictLections = notstrictLections.Where(l => l.Subject == searchDTO.Subject).ToList();
            }
            if (searchDTO.StrictProperties.IndexOf(3) != -1)
            {
                notstrictLections = notstrictLections.Where(l => l.Language == searchDTO.Language).ToList();
            }
            if (searchDTO.StrictProperties.IndexOf(4) != -1)
            {
                notstrictLections = notstrictLections.Where(l => l.Author == searchDTO.Author).ToList();
            }
            if (searchDTO.StrictProperties.IndexOf(5) != -1)
            {
                notstrictLections = notstrictLections.Where(l => l.Rating >= searchDTO.MinRating && l.Rating <= searchDTO.MaxRating).ToList();
            }
            if (searchDTO.StrictProperties.IndexOf(6) != -1)
            {
                notstrictLections = notstrictLections.Where(l => l.Year >= searchDTO.MinYear && l.Year <= searchDTO.MaxYear).ToList();
            }
            if (searchDTO.StrictProperties.IndexOf(7) != -1)
            {
                notstrictLections = notstrictLections.Where(l => l.Pages >= searchDTO.MinPages && l.Pages <= searchDTO.MaxPages).ToList();
            }
            if (searchDTO.StrictProperties.IndexOf(8) != -1)
            {
                notstrictLections = notstrictLections.Where(l => l.ThemesCount >= searchDTO.MinThemes && l.ThemesCount <= searchDTO.MaxThemes).ToList();
            }
            

            LectionCoeff[,] coeffsMatrix = new LectionCoeff[notstrictLections.Count, strictLections.Count];

            for (int i = 0; i < strictLections.Count(); i++)
            {
                for(int j = 0; j < notstrictLections.Count; j++)
                {
                    coeffsMatrix[j, i] = new LectionCoeff() { Id = notstrictLections[j].Id, 
                        Coeff = Measures.CorrelationDistance(notstrictLections[j], strictLections.ToList()[i]) * Measures.EqualValues(notstrictLections[j], strictLections.ToList()[i]) };
                }
            }

            List<LectionCoeff> coeffsList = new List<LectionCoeff>();
            for (int j = 0; j < notstrictLections.Count(); j++)
            {
                double coeff = 0;
                for (int i = 0; i < strictLections.Count(); i++)
                {
                    coeff += coeffsMatrix[j, i].Coeff;
                }
                coeffsList.Add(new LectionCoeff() { Coeff = coeff, Id = coeffsMatrix[j,0].Id });
            }

            coeffsList = coeffsList.OrderByDescending(l => l.Coeff).ToList();

            List<Lection> notstrictList = new List<Lection>();
            for (int j = 0; j < notstrictLections.Count; j++)
            {
                
                notstrictList.Add(_db.Lections.FirstOrDefault(l => l.Id == coeffsList[j].Id));
            }
            notstrictLections = notstrictList.Except(strictLections).ToList();
            foreach(Lection l in notstrictLections)
            {
                result.Add(new LectionSearchDTO()
                {
                    Title = l.Title,
                    Author = l.Author,
                    Language = l.Language,
                    Pages = l.Pages,
                    Rating = l.Rating,
                    Subject = l.Subject,
                    ThemesCount = l.ThemesCount,
                    University = l.University,
                    Year = l.Year,
                    Strict = "Not strict"
                });
            }
            return View(result);
        }
    }
}