using Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Stripe;
using Stripe.Checkout;

namespace Auth.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public CheckoutController(ApplicationDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }



        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return View("Error");
            }

            var basket = _dbContext.baskets
                .Include(p => p.Products)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(u => u.UserId == user.Id);

            if (basket == null || !basket.Products.Any())
            {
                return RedirectToAction("Index", "Basket");
            }

            var LineItems = basket.Products.Select(item => new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "gbp",
                    UnitAmount = (long)(item.Product.Price * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.Product.Name,
                    }
                },
                Quantity = item.Quantity
            }).ToList();

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = LineItems,
                Mode = "payment",
                SuccessUrl = "https://localhost:7262/Checkout/Success",
                CancelUrl = "https://localhost:7262/Checkout/Cancel",
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return Redirect(session.Url);
        }


        }

        public IActionResult Success()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

       
    }
}
