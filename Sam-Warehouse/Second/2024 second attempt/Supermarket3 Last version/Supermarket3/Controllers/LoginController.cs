using Supermarket3.Data;
using Supermarket3.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Supermarket3.Controllers
{
    public class LoginController : Controller
    {
        private readonly Supermarket3DBContext _context;
        private readonly UserRepository _userRepository;

        public LoginController(Supermarket3DBContext context, UserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public IActionResult Login([FromQuery] string ReturnUrl)
        {
            LoginDTO login = new LoginDTO
            {
                ReturnUrl = String.IsNullOrWhiteSpace(ReturnUrl) ? "/Home" : ReturnUrl
            };

            return View(login);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            //Checks if the username and account match an existing user account.
            var account = _userRepository.Authenticate(login);
            //If no match is found, return to the view.
            if (account == null)
            {
                ViewBag.LoginMessage = "Username or Password is incorrect";
                return View(login);
            }




            //Create new claim list for the current user.
            var claims = new List<Claim>
            { 
                new Claim(ClaimTypes.Name, account.Username),
                new Claim(ClaimTypes.Role, account.Role),
                //Add the user's ID to their current claims so we can retrieve it and use it to find their
                //shopping carts when we need it later.
                new Claim("ID",account.Id.ToString())
            };

            //Create new identity using the created claims.
            var claimsIdenity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                //Allows the sliding expiration cookie rule to be used on this user login.
                AllowRefresh = true,
                //Lets the cookie persist over multiple sessions within the timout period, not just the next one.
                IsPersistent = true,
                //Takes back the redirect address for once the login is completed.
                RedirectUri = login.ReturnUrl
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                                                    new ClaimsPrincipal(claimsIdenity), authProperties);
            //Store the current users id in the session so we can retrieve it when we 
            //need to identify who's shopping cart to display.
            HttpContext.Session.SetInt32("ID", account.Id);

            return Redirect(login.ReturnUrl);
        }

        public async Task<IActionResult> Logoff()
        {
            await HttpContext.SignOutAsync();
            //If the user signs out, set the session value to -1 which will be used to indicate
            //there is no user cart to display.
            HttpContext.Session.SetInt32("ID", -1);
            return RedirectToAction("Index", "Home");
        }

        
        public IActionResult AddUser()
        {
        

            return View();
        }

        [HttpPost]
        
        public IActionResult AddUser(LoginDTO login)
        {
           
           

            if (login.Password.Equals(login.PasswordConfirmation) == false)
            {
                ViewBag.NewUserMessage = "Password and confirmation do not match";
                return View();
            }

            var account = _userRepository.CreateUser(login, "Client");
            if (account == null) 
            {
                ViewBag.NewUserMessage = "Username already exists!";
                return View();
            }

            ViewBag.NewUserMessage = "New user added!";
            ModelState.Clear();

            return View();
        }
    }
}
