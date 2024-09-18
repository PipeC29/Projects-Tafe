using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TypicalTechTools.Models;

namespace TypicalTools.Controllers
{
    public class CommentController : Controller
    {
        // Creating an instance of the context class
        TypicalTechToolsDbContext _context;

        public CommentController(TypicalTechToolsDbContext commentContext)
        {
            _context = commentContext;
        }


        // Retrieves comments matching the product code from the database and sends to the view
        public IActionResult CommentList(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductCode == id);

            if (product == null)
            {
                return NotFound();
            }

            ViewBag.SessionId = HttpContext.Session.Id;

            var comments = _context.Comments.Where(c => c.ProductCode == id).OrderByDescending(c => c.CreatedDate).ToList();

            return View(comments);
        }

        // Present the user with an empty page. We use the parameter here because we need to specify
        //      which product we are commenting on
        public IActionResult AddComment(int productCode)
        {
            Comment newComment = new Comment()
            {
                CreatedDate = DateTime.Now,
                ProductCode = productCode,
                SessionId = HttpContext.Session.Id,
                //HttpContext.Session.SetString("Id",HttpContext.Session.Id.ToString())
            };
            ViewBag.SessionId = newComment.SessionId;
            return View(newComment);
        }

        // Handle the data returned (posted) by the user / from the client
        [HttpPost]
        public IActionResult AddComment(IFormCollection formCollection)
        {
            // get the comment text from the form collection
            var commentText = formCollection["CommentText"];
            int productCode = int.Parse(formCollection["ProductCode"]);
            string sessionId = formCollection["SessionId"];

            // create a new Comment object with the product code and comment text
            Comment newComment = new Comment()
            {
                CommentText = commentText,
                ProductCode = productCode,
                CreatedDate = DateTime.Now,
                SessionId = sessionId
            };
            ViewBag.SessionId = newComment.SessionId;

            // add the comment object to the CommentContext and save changes
            _context.Comments.Add(newComment);
            _context.SaveChanges();

            // redirect to the Index action
            return RedirectToAction("CommentList", new { id = productCode });
        }


        [HttpGet]
        // GET: CommentController/Edit/5


        public IActionResult EditComment(int commentId)
        {
            Comment comment = _context.Comments.FirstOrDefault(c => c.CommentId == commentId);
            if (comment == null)
            {
                return NotFound(); // Handle not found condition
            }

            return View(comment);
        }


        public IActionResult EditComment(Comment comment)
        {
            if (comment == null)
            {
                return RedirectToAction("Index", "Product");
            }

            // Check if the user is logged in
            bool isLoggedIn = HttpContext.User.Identity.IsAuthenticated;

            // Check if the user has already edited the comment
            bool hasEditedComment;

            if (HttpContext.Session.TryGetValue("HasEditedComment", out byte[] hasEditedCommentBytes))
            {
                hasEditedComment = BitConverter.ToBoolean(hasEditedCommentBytes, 0);
            }
            else
            {
                // Handle the case where the session variable doesn't exist or isn't set
                hasEditedComment = false;
            }

            // Check if the user is Guest1 and hasn't edited the comment yet
            if (isLoggedIn && !hasEditedComment)
            {
                // Perform the edit logic here
                comment.CreatedDate = DateTime.Now;


                // Mark that the user has edited the comment
                HttpContext.Session.Set("HasEditedComment", BitConverter.GetBytes(true));

                _context.Comments.Update(comment);


            }
            else if (isLoggedIn && hasEditedComment)
            {
                // User is logged in but has already edited the comment
                ModelState.AddModelError("", "You have already edited your comment once.");
            }
            else
            {
                string authStatus = HttpContext.Session.GetString("IsAuthenticated");
                bool isAdmin = !String.IsNullOrWhiteSpace(authStatus) && authStatus.Equals("true");
                comment.CreatedDate = DateTime.Now;
                Console.WriteLine("You Comment has been added");
                _context.Comments.Update(comment);
                _context.SaveChanges();

            }
            return RedirectToAction("CommentList", "Comment", new { id = comment.ProductCode });

            return View(comment);

        }



        public IActionResult DeleteComment(int? id)
        {
            //Checks if no id was sent and returns an error
            if (id == null || id <= 0)
            {
                return BadRequest();
            }
            //Requests a single author based upon the provided ID
            var author = _context.Comments.Find(id);
            //Checks if the attempted id didn't retriev an entry
            if (author == null)
            {
                return BadRequest();
            }
            //Pass the author to the view.
            return View(author);
        }


        // POST: AuthorsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteComment(int? id, Comment comment)
        {
            //Checks if no id was sent, or the id send doesn;t match the one in the model
            //If so, it returns an error
            if (id == null || id != comment.CommentId)
            {
                return BadRequest();
            }
            //Check if a record exists that matches the provided id.
            //If this check is not done, potential errors might occur.
            if (_context.Comments.Any(c => c.CommentId == id) == false)
            {
                return BadRequest();
            }
            //Flag the author to be removed fomr the databse and process the deletion
            _context.Comments.Remove(comment);
            _context.SaveChanges();
            //Redirect the controller back to the index action
            return RedirectToAction("CommentList", new { id = comment.ProductCode });
        }


        [HttpPost]
        public IActionResult DeleteCommentConfirmed(int commentId)
        {
            Comment comment = _context.Comments.FirstOrDefault(c => c.CommentId == commentId);

            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }

            return RedirectToAction("CommentList", new { id = comment.ProductCode });
        }

    }
}