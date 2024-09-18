using TypicalTechTools.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TypicalTools.Controllers
{
    public class AdminController : Controller
    {
        private readonly TypicalTechToolsDbContext _context;


        public AdminController(TypicalTechToolsDbContext context)
        {
            _context = context;
        }
        public IActionResult AdminLogin()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AdminLogin(LoginUserDTO user)
        {
            var account = _context.AdminUsers.Where(a => a.UserName == user.UserName).FirstOrDefault();
            if (account == null)
            {
                ViewBag.LoginError = "Login Error,Please Try Again";
                return View(user);
            }

            if (BCrypt.Net.BCrypt.EnhancedVerify(user.Password, account.PasswordHarsh))
            {
                HttpContext.Session.SetString("Id",user.UserName.ToString());
                HttpContext.Session.SetString("IsAuthenticated", "true");

                return RedirectToAction("Index", "Product");
            }

            ViewBag.LoginError = "Login Error,Please Try Again";

            return View(user);
        }
        public IActionResult LogOff()
        {
            HttpContext.Session.SetString("IsAuthenticated", "false");
            return RedirectToAction("Index", "Product");
        }

        public IActionResult CreateUser()
        {
            string authenticated = HttpContext.Session.GetString("IsAuthenticated") ?? "false";
            if (authenticated.Equals("false")) 
            {
                ViewBag.LoginError = "Login required to access the page.";

                return View("Login");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult CreateUser(CreateUserDTO user)
        {
            if (!ModelState.IsValid)
            {
                // If the model is not valid, return to the same view with validation errors.
                return View(user);
            }

            // Check if the username is already taken
            if (_context.AdminUsers.Any(a => a.UserName == user.UserName))
            {
                // Create a message and return that the username is already taken.
                ModelState.AddModelError("UserName", "Username already exists.");
                return View(user);
            }

            // Check that the password and confirmation match
            if (user.Password != user.PasswordConfirmation)
            {
                // Create a message and return if not matching
                ModelState.AddModelError("PasswordConfirmation", "Password and Password confirmation do not match");
                return View(user);
            }

            // Create a new admin user object and fill it out using bcrypt
            var newUser = new AdminUser
            {
                UserName = user.UserName,
                PasswordHarsh = BCrypt.Net.BCrypt.EnhancedHashPassword(user.Password)
            };

            // Add the new user to the database and save it.
            _context.AdminUsers.Add(newUser);
            _context.SaveChanges();

            return RedirectToAction("HomeIndex", "Home");
        }

    }

}



