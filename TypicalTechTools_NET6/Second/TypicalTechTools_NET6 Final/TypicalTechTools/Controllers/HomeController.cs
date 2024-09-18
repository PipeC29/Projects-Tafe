using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TypicalTechTools.Models;

namespace TypicalTechTools.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult HomeIndex()
        {
            // Get the session values stored in the user session if they exist.
            // If not, set the variables to default values.
            string username = HttpContext.Session.GetString("Username");
            int viewed = HttpContext.Session.GetInt32("Viewed") ?? 0;

            if (!string.IsNullOrEmpty(username))
            {
                // Increment the 'viewed' count for logged-in users.
                viewed++;
            }
            else
            {
                // If the user is not logged in, set a default username (e.g., "Guest").
                username = "Guest";
            }

            // Store the updated values in the session.
            HttpContext.Session.SetString("Username", username);
            HttpContext.Session.SetInt32("Viewed", viewed);

            // Pass the username and viewed count to the view.
            ViewData["Username"] = username;
            ViewData["Viewed"] = viewed;

            return View();
        }


        public IActionResult Privacy()
        {
            //Goes to the user session and sets 2 values in it
            //One string and one integer
            //Each set of data works off a Key:Value principle with the first parameter.
            //being the key for the data
            HttpContext.Session.SetString("Name", "User");
            HttpContext.Session.SetInt32("Viewed", 0);
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}