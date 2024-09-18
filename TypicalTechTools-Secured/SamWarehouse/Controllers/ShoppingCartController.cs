using BookStoreApp.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using BookStoreApp.Models;
using System.Collections.Generic;

namespace BookStoreApp.Controllers
{
    public class ShoppingCartController : Controller
    {
        public readonly BookStoreDBContext _context;

        public ShoppingCartController(BookStoreDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            //Get the user id out of the session ansd store it.
            var id = HttpContext.Session.GetInt32("ID");
            //Check if the user is not logged in and an ivalid ID has been stored.
            if (id == null || id < 0 || HttpContext.User.Identity.IsAuthenticated == false)
            {
                return BadRequest();
            }

            //Get the shopping cart for the current user from the database.
            var shoppingCart = _context.ShoppingCarts.Include(c => c.CartUser)
                .Where(c => c.UserId == id && c.IsFinalised == false).FirstOrDefault();


            //If the user doesnt currently have an open cart returnt to the partial view with
            //no data provided
            if (shoppingCart == null)
            {
                return PartialView("_ShoppingCartPartial");
            }
            {
                
            }

            //Get the shopping cart lines for the selected cart and add them to its cart items.
            shoppingCart.CartItems = _context.ShoppingCartItems.Include(ci => ci.BookItem)
                .Where(ci => ci.ShoppingCartId == shoppingCart.Id).ToList();

            //Return the shopping cart partial view with any data we hand over to it.
            return PartialView("_ShoppingCartPartial",shoppingCart);
        }

        public  async Task<ActionResult> UpdateQuantity([FromBody] ShoppingCartItem item)
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
            //Save the changes to the database.
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
            //Find all the cart items for the cart with the ID we have provided. Also get their books
            //so we can get the book price for each item.
            var cartItems = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == id)
                                        .Include(c => c.BookItem).ToList();

            //Create a variable to hold the total of our cart.
            double total = 0;
            //Cycle through all the items for the cart
            foreach (var item in cartItems)
            {
                //Multiply the book price by the quantity for each item and add it to the current total.
                total += (double)item.BookItem.Price * item.Quantity;
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
                BookId = Id,
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
                var item = cart.CartItems.Where(ci => ci.BookId == Id).FirstOrDefault();

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

    }
}
