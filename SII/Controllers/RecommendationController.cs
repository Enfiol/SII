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

        [HttpGet("lections")]
        public IActionResult Lections()
        {
            var lections = _db.Lections.ToList();
            return View(lections);
        }

        //[HttpGet("similarlections")]
        //public IActionResult SimilarLections()
        //{
           
        //    return View();
        //}
    }
}