using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practica9.Data;

namespace Practica9.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {

        private readonly IdentityDbContext _context;

        public CustomerController(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var customers = await _context.Customers.Include(c => c.Orders).ToListAsync(); // SELECT * FROM Customers
            return View(customers);
        }
    }
}
