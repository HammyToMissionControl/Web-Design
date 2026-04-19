using Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Auth.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _webHost;

        public ProductController(ApplicationDbContext dbContext, IWebHostEnvironment webHost)
        {
            _dbContext = dbContext;
            _webHost = webHost;
        }

        private void Populate()
        {
            ViewBag.Categories = new SelectList(_dbContext.categories, "Id", "Name");
        }

        public IActionResult Index()
        {
            var products = _dbContext.products.Include(p => p.Category).ToList();

            return View(products);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult editProduct(int id)
        {
            var product = _dbContext.products.Find(id);

            if (product == null)
            {
                return View("Error");
            }

            Populate();
            return View(product);
        }


        [Authorize (Roles ="Admin")]
        public IActionResult createProduct()
        {
            Populate();
            return View();
        }

        [Authorize(Roles = "Admin")]

        [HttpPost]
        public async Task<IActionResult> createProduct(Product product, IFormFile ImageFile)
        {

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                var savePath = Path.Combine(_webHost.WebRootPath, "images", fileName);

                using (var stream = new FileStream(savePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                product.ImagePath = "/images/" + fileName;
            }


            if (product.CategoryId == 0)
            {
                ModelState.AddModelError("CategoryId", "Please select a category");
            }

            if (ModelState.IsValid)
            {
                _dbContext.Add(product);

                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Index");

            }

            Populate();
            return View(product);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> editProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                _dbContext.Update(product);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            Populate();

            return View(product);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> deleteProduct(int id)
        {
            if (ModelState.IsValid)
            {
                var product = _dbContext.products.Find(id);


                _dbContext.Remove(product);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");

            }

            Populate();
            return RedirectToAction("editProduct");
        }
    }
}
