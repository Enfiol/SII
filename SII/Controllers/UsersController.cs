using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SII.Controllers
{
    [Route("users")]

   
    public class UsersController : Controller
    {
        private readonly ILogger<UsersController> _logger;
        private readonly ApplicationContext _db;

        public UsersController(ILogger<UsersController> logger, ApplicationContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult Index()
        {
            List<User> list = _db.Users.ToList();
            return View(list);
        }

        [HttpPost]
        public IActionResult Index(User newUser)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == newUser.Id);
            if(user != null)
            {
                return Conflict();
            }
            _db.Users.Add(newUser);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(int id)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet("clearstat/{id}")]
        public IActionResult ClearStat(int id)
        {
            User user = _db.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            List<UserLection> ulections = _db.UserLections.Where(ul => ul.UserId == id).ToList();
            foreach(UserLection ul in ulections)
            {
                _db.UserLections.Remove(ul);
            }
            _db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}