using INTEX2._0.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace INTEX2._0.Controllers
{
    public class AdminController : Controller
    {
        private IIntexRepository _repo;
        private IUsers _usersRepo; // Add IUsers dependency

        public AdminController(IIntexRepository temp, IUsers usersRepo) // Include IUsers in the constructor
        {
            _repo = temp;
            _usersRepo = usersRepo; // Assign IUsers dependency
        }

        public IActionResult Users()
        {
            var users = _usersRepo.AspNetUsers.ToList(); // Fetch users from your repository
            ViewBag.Users = users;
            return View();
        }

        public IActionResult Products()
        {
            ViewBag.Products = _repo.Products.ToList();
            ViewBag.Categories = _repo.Categories.ToList();
            ViewBag.Colors = _repo.Products.Select(p => p.PrimaryColor).Distinct();
            return View();
        }

        public IActionResult Orders()
        {
            var fraudOrders = _repo.Orders.Where(x => x.Fraud == 1).OrderBy(x => x.Date).ToList();
            ViewBag.Orders = fraudOrders; // Set ViewBag.Orders with the list of fraud orders
            return View();
        }
    }
}
