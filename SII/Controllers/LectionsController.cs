using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SII.Controllers
{
    [Route("lections")]
    public class LectionsController : Controller
    {
        private readonly ILogger<LectionsController> _logger;
        private readonly ApplicationContext _db;

        public LectionsController(ILogger<LectionsController> logger, ApplicationContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<Lection> lections = _db.Lections.ToList();
            return View(lections);
        }

        [HttpPost]
        public IActionResult Index(Lection newLection)
        {
            Lection lection = _db.Lections.FirstOrDefault(l => l.Id == newLection.Id);
            if (lection != null)
            {
                return Conflict();
            }
            _db.Lections.Add(newLection);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            Lection lection = _db.Lections.FirstOrDefault(l => l.Id == id);
            if(lection==null)
            {
                return NotFound();
            }
            _db.Lections.Remove(lection);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}