using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using csharp_exam.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace csharp_exam.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private MyContext _context;

        public HomeController(ILogger<HomeController> logger, MyContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User newUser)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u=>u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                _context.Users.Add(newUser);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            } else {
                return View("Index");
            }
        }

        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            int? validUser = HttpContext.Session.GetInt32("UserId");
            if(validUser == null)
            {
                ModelState.AddModelError("LEmail", "You must first login to access web pages.");
                return View("Index");
            } else {
            ViewBag.userInfo = HttpContext.Session.GetInt32("UserId");
            List<Act> allActivities = _context.Activities.Include(b=>b.Participants).ThenInclude(b=>b.Participant).OrderBy(d=>d.Date).ThenBy(d=>d.Time).ToList();
            return View(allActivities);
            }
        }

        [HttpPost("login")]
        public IActionResult Login(LogUser logUser)
        {
            
            if(ModelState.IsValid)
            {
                User userInDB = _context.Users.FirstOrDefault(s=>s.Email == logUser.LEmail);
                if(userInDB == null)
                {
                    ModelState.AddModelError("LEmail", "Invalid login attempt");
                    return View("Index");
                }
                PasswordHasher<LogUser> Hasher = new PasswordHasher<LogUser>();
                PasswordVerificationResult result = Hasher.VerifyHashedPassword(logUser, userInDB.Password, logUser.LPassword);
                if(result == 0)
                {
                    ModelState.AddModelError("LEmail", "Invalid login attempt");
                    return View("Index");
                }
                HttpContext.Session.SetInt32("UserId", userInDB.UserId);
                return RedirectToAction("Dashboard");
            } else {
                return View("Index");
            }
        }

        [HttpGet("/addActivity")]
        public IActionResult addActivity()
        {
            ViewBag.userInfo = HttpContext.Session.GetInt32("UserId");
            return View("NewActivity");
        }

        [HttpPost("/createActivity")]
        public IActionResult createActivity(Act newAct)
        {
            if(newAct.Date < DateTime.Now)
            {
                ModelState.AddModelError("Date", "Date cannot be a past date!");
            }
            if(ModelState.IsValid)
            {
                _context.Activities.Add(newAct);
                _context.SaveChanges();
                return RedirectToAction("Dashboard");
            } else {
                return View("NewActivity");
            }
        }

        [HttpGet("/view/activity/{aid}")]
        public IActionResult ViewActivity(int aid)
        {
            ViewBag.actInfo = _context.Activities.Include(a=>a.Creator).FirstOrDefault(a=>a.ActId == aid);
            Act listAct = _context.Activities.Include(a=>a.Participants).ThenInclude(a=>a.Participant).FirstOrDefault(a=>a.ActId == aid);
            ViewBag.userInfo = HttpContext.Session.GetInt32("UserId");
            return View("ViewActivity", listAct);
        }

        [HttpGet("/join/{aid}/{uid}")]
        public IActionResult JoinEvent(int uid, int aid)
        {
            Association newAssn = new Association();
            newAssn.ActId = aid;
            newAssn.UserId = uid;
            _context.associations.Add(newAssn);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("/delete/{aid}")]
        public IActionResult DeleteEvent(int aid)
        {
            Act removedAssn = _context.Activities.SingleOrDefault(a=>a.ActId == aid);
            _context.Activities.Remove(removedAssn);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("/leave/{aid}/{uid}")]
        public IActionResult LeaveEvent(int aid, int uid)
        {
            Association removedAssn = _context.associations.SingleOrDefault(a=>a.UserId==uid && a.ActId==aid);
            _context.associations.Remove(removedAssn);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
