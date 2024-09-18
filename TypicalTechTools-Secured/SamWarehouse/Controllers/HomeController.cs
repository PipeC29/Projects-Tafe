using BookStoreApp.Models;
using BookStoreApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly FileUploaderService _uploader;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, FileUploaderService uploader)
        {
            _logger = logger;
            _uploader = uploader;
        }

        public IActionResult Index()
        {
            return View();
        }

        //[Authorize]
        public IActionResult Privacy()
        {
            return View();
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> ImageUpload(IFormFile file)
        {
            _uploader.SaveFile(file);
            return View("Index");
        }

        [HttpPost]
        public IActionResult DownloadFile(string fileName)
        {
            //Get the file contents from the file uploader system
            byte[] fileData = _uploader.DownloadFile(fileName);
            //If the file didn't exist, redirect back to the home page.
            if (fileData == null)
            {
                return RedirectToAction("Index");
            }
            //Return the file to the user's browser
            return File(fileData, "application/octet-stream", fileDownloadName: fileName);
        }
    }
}
