using TypicalTechTools.DataAccess;
using TypicalTechTools.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace TypicalTools.Controllers
{
   
    public class ProductController : Controller
    {
        private readonly TypicalTechToolsDbContext _context;
        private readonly ILogger<ProductController> _logger;
        private readonly Product _products;
      
        public ProductController(TypicalTechToolsDbContext context) 
        {
            //_csvParser = parser;
            //_logger = logger;
            _context = context;
                   
        }


        //POST: Products/SearchResults
        public IActionResult ShowSearchResults(string searchPhrase)
        {
         
            {
                // Like an if else statement.
                string search = string.IsNullOrWhiteSpace(searchPhrase) ? "" : searchPhrase;
                // Stores the search term in the session so that we can put it back when needed.
                HttpContext.Session.SetString("LastProductSearch", search);


                // Request the products from the database using the context class.
                var products = _context.Products.Where(predicate: p => p.ProductName.Contains(search)).ToList();

              
                // Pass the product collection to the view.
                //return RedirectToAction(nameof(Index));
              return View("Index", products);
            }
            //return RedirectToAction(nameof(Index));

        }


        [HttpGet]
        // Displays all the products
        public IActionResult Index()
        {
            // var products = _context.Products.ToList();

            // Retrieve products from the database context as an enumerable collection.
            var products = _context.Products.AsEnumerable();

            // Return the "products" collection to the view.
            return View(products);
        }


        // This action renders the search form view.
        // GET: /ControllerName/SearchForm
        public async Task<IActionResult> SearchForm()
        {
            return View();
        }



        //POST: Products/SearchResults
        public async Task<IActionResult> SearchResults(String? searchPhrase)
        {
            {
                return View("Index", await _context.Products.Where(p => p.ProductName.Contains(searchPhrase)).ToListAsync());
            }
            return View("Index");
        }


        [HttpGet]
        //THis action get a single Product by Id.
        //GET:ProductController/Details
        public IActionResult Details(int id)
        {
            //Checks if no id was sent and returns an error
            if (id == null || id <= 0)
            {
                return BadRequest();
            }
            //Request a single product based upon the provided ID.
            var products = _context.Products.Find(id);
            //var product = _product.Products.FirstOrDefault(p => p.Id == id);

            //Check if the attempted id didn't retrieve an entry
            if (products == null)
            {
                return BadRequest();
            }
            
            return View(products);
        }


        [HttpGet]
        //GET:ProductController/Create
        public ActionResult Create()
        {
            return View();
        }

        //Post:ProductController/Create
        // This is the method that gets executed when the form for creating a new product is submitted.
        // It specifies that this action only accepts HTTP POST requests.
        
        [HttpPost]
        // A security feature to prevent cross-site request forgery attacks.
        [ValidateAntiForgeryToken]


        public ActionResult Create(Product products)
        {
            // Remove the "Comment" property from the ModelState to avoid validation errors related to it.
            ModelState.Remove("Comment");
            // Check if the submitted data matches the validation rules defined in the Product model.
            //System/Class that looks at the properties that are being provide and the class and match
            if (ModelState.IsValid)
            {
                // If the submitted data is valid, add the product to the database.
                _context.Products.Add(products);

                // Save the changes to the database
                _context.SaveChanges();
                TempData["SuccessMessage"] = "New entry has been added successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }


        [HttpGet]
        //GET:ProductController/Edit
        public IActionResult Edit(int? id) 
        {
            //Checks if no id was sent and returns an error
            if (id == null || id <= 0)
            {
                return BadRequest();
            }
            //Request a single products based upon the provided ID.
            var products = _context.Products.Find(id);

            //Check if the attempted id didn't retrieve an entry
            if (products == null)
            {
                return BadRequest();
            }
            //Pass the products to the view.
            return View(products);
        }
        

        // POST: ProductController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int? id, Product product)
        {
            if (id != product.ProductCode)
            {
                return BadRequest();
            }

            var existingProduct = _context.Products.Any(p => p.ProductCode == id);

            if (existingProduct == null)
            {
                return BadRequest();
            }

            ModelState.Remove("Comment");

            // Set the UpdatedDate before marking the entity as modified
            //existingProduct.UpdatedDate = DateTime.Now;

            // You don't need to call _context.Products.Update(products) if 'products' is already being tracked by Entity Framework

            if (ModelState.IsValid)
            {
                // Find the existing product by its unique identifier, e.g., ProductCode
                var editedProduct = _context.Products.FirstOrDefault(p => p.ProductCode == product.ProductCode);

               

                // Update the properties of the existing product
                editedProduct.ProductName = product.ProductName;
                editedProduct.ProductDescription = product.ProductDescription;
                editedProduct.ProductPrice = product.ProductPrice;
                editedProduct.UpdatedDate = DateTime.Now;

                // Save changes to the database
                _context.SaveChanges();

                return RedirectToAction(nameof(Index));
            }


            return View(product);
        }
        

        [HttpGet]
        //GET:ProductController/Delete
        public IActionResult Delete(int? id)
        {
            //Checks if no id was sent and returns an error
            if (id == null || id <= 0)
            {
                return BadRequest();
            }
            //Request a single author based upon the provided ID.
            var products = _context.Products.Find(id);

            //Check if the attempted id didn't retrieve an entry
            if (products == null)
            {
                return BadRequest();
            }
            //Pass the author to the view.
            return View(products);
        }


        // POST: ProductController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int? id, Product products)
        {
            if (id == null || id != products.ProductCode)
            {
                return BadRequest();
            }

            if (_context.Products.Any(a => a.ProductCode == id) == false)
            {
                return BadRequest();
            }
            _context.Products.Remove(products);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult NewProduct()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult NewProduct(Product products)
        {
            // Remove the "Comment" property from the ModelState to avoid validation errors related to it.
            ModelState.Remove("Comment");
            // Check if the submitted data matches the validation rules defined in the Product model.
            //System/Class that looks at the properties that are being provide and the class and match
            if (ModelState.IsValid)
            {
                // If the submitted data is valid, add the product to the database.
                _context.Products.Add(products);

                // Save the changes to the database
                _context.SaveChanges();
                TempData["SuccessMessage"] = "New entry has been added successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }


        public IActionResult Privacy()
        {
            return View();
        }
    }
}
