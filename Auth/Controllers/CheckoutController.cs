using Auth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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


            return View();
        }
    }
}
