using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SII.Controllers
{
    [Route("marks")]
    public class MarksController : Controller
    {
        private readonly ILogger<MarksController> _logger;
        private readonly ApplicationContext _db;

        public MarksController(ILogger<MarksController> logger, ApplicationContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<UserMark> list = _db.UserMarks.ToList();
            return View(list);
        }

        [HttpPost]
        public IActionResult Index(UserMark newUserMark)
        {
            UserMark userMark = _db.UserMarks.FirstOrDefault(um => um.Id == newUserMark.Id);
            if (userMark != null)
            {
                return Conflict();
            }
            _db.UserMarks.Add(newUserMark);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            UserMark userMark = _db.UserMarks.FirstOrDefault(um => um.Id == id);
            if (userMark == null)
            {
                return NotFound();
            }
            _db.UserMarks.Remove(userMark);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}