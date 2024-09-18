using Supermarket3.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Supermarket3.Models;
using System.Collections.Generic;
using System;
using System.Security.Claims;

namespace Supermarket3.Controllers
{
    public class ShoppingCartController : Controller
    {
        public readonly Supermarket3DBContext _context;

        public ShoppingCartController(Supermarket3DBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Ensure the user is authenticated before accessing session data
            if (!HttpContext.User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var id = HttpContext.Session.GetInt32("ID");
            if (id == null || id < 0)
            {
                return BadRequest();
            }

            var shoppingCart = _context.ShoppingCarts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.ProductItem)
                .FirstOrDefault(c => c.UserId == id && !c.IsFinalised);

            if (shoppingCart == null)
            {
                return PartialView("_ShoppingCartPartial");
            }

            return PartialView("_ShoppingCartPartial", shoppingCart);
        }


        public async Task<ActionResult> UpdateQuantity([FromBody] ShoppingCartItem item)
        {
            //Pass the model to Entity Framework to be used. By using the attach command
            //we can tell entity framework to only change the filed we specify in the next step.
            _context.ShoppingCartItems.Attach(item);

            //Tell entity framework to find the Quantity property of the item
            //and sets the IsModified  property to true. This will mark this property
            //as one that needs to be updated in the database.
            _context.Entry(item).Property(x => x.Quantity).IsModified =true;


            await _context.SaveChangesAsync();
            return Ok();

        }

        public async Task<ActionResult> FinaliseCart(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }
            //Create a new cart object with the ID, Total and isFinalised properties set.
            //The toal will be set by calling the calculate total method and assigning the result.
            var cart = new ShoppingCart { Id = id, Total = CalculateCartTotal(id), IsFinalised = true };

            //Pass the new cart model to the context class for saving.
            _context.ShoppingCarts.Attach(cart);
            //Tell the context class which properties we want to update
            _context.Entry(cart).Property(c => c.IsFinalised).IsModified = true;
            _context.Entry(cart).Property(c => c.Total).IsModified = true;
            _context.Entry(cart).Property(c => c.Date).CurrentValue = DateTime.Now;
            //Save the changes to the database.
            await _context.SaveChangesAsync();

            return Ok();
        }

        public async Task<ActionResult> ClearCart(int id)
        {
            if (id < 1)
            {
                return BadRequest();
            }

            // Get the shopping cart for the given ID
            var cart = await _context.ShoppingCarts.Where(c => c.Id == id)
                                       .Include(c => c.CartItems)
                                       .FirstOrDefaultAsync();

            if (cart == null)
            {
                return NotFound(); // Return NotFound if cart doesn't exist
            }

            // Clear the existing cart items
            cart.CartItems.Clear();

            // Update the cart in the context (no properties need to be explicitly marked as modified)
            _context.ShoppingCarts.Update(cart);

            // Save the changes to the database
            await _context.SaveChangesAsync();

            return Ok();
        }


        public async Task<ActionResult> RemoveFromCart([FromBody] ShoppingCartItem item)
        {
            // _context.Remove(_context.ShoppingCartItems.Single(ci => ci.Id == item.Id));
            _context?.ShoppingCartItems.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }

        public double CalculateCartTotal(int id)
        {
            //Find all the cart items for the cart with the ID we have provided. Also get their items
            //so we can get the item price for each item.
            var cartItems = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == id)
                                        .Include(c => c.ProductItem).ToList();

            //Create a variable to hold the total of our cart.
            double total = 0;
            //Cycle through all the items for the cart
            foreach (var item in cartItems)
            {
                //Multiply the item price by the quantity for each item and add it to the current total.
                total += (double)item.ProductItem.Price * item.Quantity;
            }

            return total;
        }


        public async Task<ActionResult> AddToCart(int Id)
        {
            //Chek if there is user ID stored in the session data. This will
            //default the ID to -1 if there isint one saved in the session
            var userId = HttpContext.Session.GetInt32("ID") ?? -1;

            //Check if the user is logged in and the ID is valid.
            if(userId < 0 || HttpContext.User.Identity.IsAuthenticated == false)
            {
                return Unauthorized();
            }

            var cart = _context.ShoppingCarts.Where(c => c.UserId == userId && c.IsFinalised == false)
                                                       .Include(c => c.CartItems).FirstOrDefault();

            //Create a new cart item with the details we will need to add it later when its needed.
            var cartItem = new ShoppingCartItem
            {
                ItemId = Id,
                Quantity = 1
            };

            //If the user currently doesnt have an open cart
            if(cart == null)
            {
                //Create a new cart with the users id and start a new cart item list with the
                //created item as its first entry.
                cart = new ShoppingCart
                {
                    UserId = userId,
                    CartItems = new List<ShoppingCartItem> { cartItem }
                };

                //Add the new cart to Entity Framwork as well as the details of the new cartItem from within
                //the cart. This will also add the cartItem's foreign key reference to the shopping cart.
                _context.ShoppingCarts.Add(cart);

            }

            else
            {
                //Check if the cart already has a copy of the item in it.
                var item = cart.CartItems.Where(ci => ci.ItemId == Id).FirstOrDefault();

                if (item != null)
                {
                    //Increase the items quantity by one
                    item.Quantity++;

                    //Hand the item over the entity framwork.
                    _context.ShoppingCartItems.Attach(item);

                    //Tell entity framework to find the Quantity property of the item
                    //and sets the IsModified  property to true. This will mark this property
                    //as one that needs to be updated in the database.
                    _context.Entry(item).Property(x => x.Quantity).IsModified = true;
                }

                else
                {
                    //Add the cart;s ID to the shopping cart item
                    cartItem.ShoppingCartId = cart.Id;
                    //Pass the cart item to entity framework to be saved
                    _context.Add(cartItem);
                }
            }

            //Save the change to the database
            await _context.SaveChangesAsync();

            //Send OK response to the user
            return Ok();
        }



        public IActionResult FullHistory()
        {
            //Find the ID claim value in the users login and retrieve its value. Return -1 if no value found.
            var value = HttpContext.User.FindFirstValue("ID") ?? "-1";
            //Convert the ID value from text back to a number.
            var id = int.Parse(value);
            //If the user is not logged in, meaning the id is equal to -1
            
            //Find the latest cart for this user
            var shoppingCarts = _context.ShoppingCarts.Where(c => c.UserId == id && c.Date != null).ToList();
            if (shoppingCarts.Count() > 0)
            {
                foreach (var cart in shoppingCarts)
                {
                    //Get the cart items and item details for each item in the current cart
                    cart.CartItems = _context.ShoppingCartItems.Include(ci => ci.ProductItem).Where(ci => ci.ShoppingCartId == cart.Id)
                                                                       .ToList();
                }
            }

            return View(shoppingCarts);
        }

        public IActionResult History()
        {
            //Find the ID claim value in the users login and retrieve its value. Return -1 if no value found.
            var value = HttpContext.User.FindFirstValue("ID") ?? "-1";
            //Convert the ID value from text back to a number.
            var id = int.Parse(value);
            //If the user is not logged in, meaning the id is equal to -1
           
            //Find the latest cart for this user
            var shoppingCarts = _context.ShoppingCarts.Where(c => c.UserId == id && c.Date != null).ToList();

            return View(shoppingCarts);
        }


        public IActionResult HistoryDetails(int id)
        {
            //Find the cart matching the provided id (cart id)
            var shoppingCart = _context.ShoppingCarts.Where(c => c.Id == id).FirstOrDefault();

            //Find the ID claim value in the current users login and retrieve its value. Return -1 if no value found.
            var value = HttpContext.User.FindFirstValue("ID") ?? "-1";
            //Convert the ID value from text back to a number.
            var userId = int.Parse(value);
            //If the user is not logged in, meaning the id is equal to -1 or their Id is does not match the AppUserId
            //in the cart, redirect to the Access denied page.
            
            //If the users  is not null.
            if (shoppingCart != null)
            {
                //Get the cart items and item details for each item in the current cart
                shoppingCart.CartItems = _context.ShoppingCartItems.Include(ci => ci.ProductItem).Where(ci => ci.ShoppingCartId == shoppingCart.Id).ToList();
            }
            return View(shoppingCart);
        }
    }
}
