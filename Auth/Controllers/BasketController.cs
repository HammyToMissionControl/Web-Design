using Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Auth.Controllers
{
    [Authorize]
    public class BasketController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        public BasketController(ApplicationDbContext dbContext)
        {

            _dbContext = dbContext;

        }

        public async Task<IActionResult> Index()
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var basket = _dbContext.baskets
                .Include(p => p.Products)
                .ThenInclude(o => o.Product)
                .FirstOrDefault(u => u.UserId == user);

            if (basket == null)
            {
                basket = new Basket
                {
                    id = Guid.NewGuid(),
                    UserId = user,
                    Products = new List<BasketItem>()
                };
                _dbContext.baskets.Add(basket);
            }
            await _dbContext.SaveChangesAsync();

            return View(basket);
        }

        [HttpPost]
        public async Task<IActionResult> AddToBasket(int productId, int quantity)
        {
            var user = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var prId = _dbContext.products.Find(productId);

            if (prId == null)
            {
                return View("Error");
            }
            if (quantity > prId.Quantity)
            {
                TempData["Error"] = $"Error message";
                return View("Index", "Product");
            }

            var basket = _dbContext.baskets.FirstOrDefault(p => p.UserId == user);

            if (basket == null)
            {
                basket = new Basket
                {
                    id = Guid.NewGuid(),
                    UserId = user,
                    Products = new List<BasketItem>()
                };
                _dbContext.baskets.Add(basket);
            }

            var item = _dbContext.items.FirstOrDefault(i => i.ProductId == productId);

            if (item != null)
            {
                item.Quantity += quantity;
                prId.Quantity -= quantity;
            }
            else
            {
                item = new BasketItem
                {
                    BasketID = basket.id,
                    ProductId = productId,
                    Quantity = quantity
                };

                prId.Quantity -= quantity;
                _dbContext.items.Add(item);
            }
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        

    }
}
