using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WeddingPlanner.Contexts;
using WeddingPlanner.Models;

namespace WeddingPlanner.Controllers
{
    public class HomeController : Controller
    {
        private Contexts.MyContext _context;
        public HomeController(MyContext context)
        {
            _context = context;
        }
        private User GetUserInDb()
        {
            return _context.Users.FirstOrDefault( u => u.UserId == HttpContext.Session.GetInt32("UserId"));
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("register")]
        public IActionResult Register(User reg)
        {
            if(ModelState.IsValid)
            {
                if(_context.Users.Any(u => u.Email == reg.Email))
                {
                    ModelState.AddModelError("Email","Email already taken");
                    return View("Index");
                }
                else
                {
                    PasswordHasher<User> Hasher = new PasswordHasher<User>();
                    reg.Password = Hasher.HashPassword(reg, reg.Password);
                    _context.Users.Add(reg);
                    _context.SaveChanges();
                    HttpContext.Session.SetInt32("UserId", reg.UserId);
                    return RedirectToAction("Dashboard");
                }
            }
            else
            {
                return View("Index");
            }
        }
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            User userInDb = GetUserInDb();
            if(userInDb == null)
            {
            return Redirect("/");
            }
            else
            {
                ViewBag.User = userInDb;
                List<Wedding> AllWeddings = _context.Weddings
                                                    .Include(w => w.myWedding)
                                                    .Include(w =>w.guests)
                                                    .ThenInclude(w => w.guest)
                                                    .Where(w => w.WeddingDay >= DateTime.Now)
                                                    .OrderBy(w => w.WeddingDay)
                                                    .ToList();
                return View("Dashboard", AllWeddings);
            }
        }
        [HttpPost("loginuser")]
        public IActionResult Login(LoginUser user)
        {
            if(ModelState.IsValid)
            {
                User userMatchingEmail = _context.Users
                    .FirstOrDefault(u => u.Email == user.LoginEmail);
                if(userMatchingEmail == null)
                {
                    ModelState.AddModelError("LoginEmail", "Unknown Email!");
                }
                else
                {
                    PasswordHasher<LoginUser> Hasher = new PasswordHasher<LoginUser>();
            
            // verify provided password against hash stored in db
                    var result = Hasher.VerifyHashedPassword(user, userMatchingEmail.Password, user.LoginPassword);
                    if(result == 0)
                    {
                        ModelState.AddModelError("LoginPassword", "Incorrect Password!");

                    }
                    else{
                        HttpContext.Session.SetInt32("UserId", userMatchingEmail.UserId);
                        return RedirectToAction("Dashboard");
                    }
                }
            }
            return View("Index");
        }
        [HttpGet("logout")]
        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [HttpGet("planWedding")]
        public IActionResult planWedding()
        {
            return View("PlanWedding");
        }

        [HttpPost("addWedding")]
        public IActionResult CreateWedding(Wedding newWedding)
        {
            User current = GetUserInDb();
            if(current == null)
            {
                return View("PlanWedding");
            }
            if(ModelState.IsValid)
            {
                newWedding.UserId = current.UserId;
                _context.Weddings.Add(newWedding);
                _context.SaveChanges();
                return View("");
            }
            return View("PlanWedding");
        }
        [HttpGet("show/{WeddingId}")]
        public IActionResult viewWedding(int WeddingId)
        {
            User current = GetUserInDb();
            if(current == null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                Wedding thiswedding = _context.Weddings
                                            .Include(w => w.guests)
                                            .ThenInclude(a => a.guest)
                                            .Include(w => w.myWedding)
                                            .FirstOrDefault(w => w.WeddingId == WeddingId);
                ViewBag.User = current;
                return View("ShowWedding", thiswedding);
            }
        }
        [HttpGet("rsvp/{WeddingId}")]
        public IActionResult RSVP(int WeddingId)
        {
            User current = GetUserInDb();
            if(current ==null)
            {
                RedirectToAction("Index");
            }
            Association going = new Association();
            going.WeddingId = WeddingId;
            going.UserId = current.UserId;
            _context.Add(going);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("notgoing/{WeddingId}")]
        public IActionResult NotGoing(int WeddingId)
        {
            User current = GetUserInDb();
            if(current == null)
            {
                return RedirectToAction("Index");
            }
            Association notgoing = _context.Associations
                                        .FirstOrDefault(w => w.WeddingId == WeddingId && w.UserId == current.UserId);
            _context.Associations.Remove(notgoing);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpGet("cancel/{WeddingId}")]
        public IActionResult Cancel(int WeddingId)
        {
            User current = GetUserInDb();
            if(current == null)
            {
                return RedirectToAction("Index");
            }
            Wedding delete = _context.Weddings
                                    .FirstOrDefault(w => w.WeddingId == WeddingId);
            _context.Weddings.Remove(delete);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }      
}
