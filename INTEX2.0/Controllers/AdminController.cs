using INTEX2._0.Models;
using INTEX2._0.Models.ViewModels;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
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
            // Fetch users from the database along with their roles
            var usersQuery = from u in _usersRepo.AspNetUsers
                             join ur in _usersRepo.AspNetUserRoles on u.Id equals ur.UserId
                             join r in _usersRepo.AspNetRoles on ur.RoleId equals r.Id
                             select new { u.UserName, r.Name, UserID = u.Id, RoleID = r.Id };

            var userData = usersQuery.ToList();

            ViewBag.Users = userData;

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
            var fraudOrders = _repo.Orders
                .Where(x => x.Fraud == 1)
                .OrderByDescending(x => x.Date)
                .Take(20)
                .ToList();
            
            return View(fraudOrders);
        }
        
        public IActionResult OrderDetails(int id)
        {
            var orderToDisplay = _repo.Orders
                .Where(x => x.TransactionId == id)
                .FirstOrDefault();
            
            return View(orderToDisplay);
        }

        public IActionResult EditProduct(int id)
        {
            return View("AddProduct");
        }

        [HttpGet]
        public IActionResult UpdateUser(string id)
        {
            var usersQuery = (from u in _usersRepo.AspNetUsers
                             join ur in _usersRepo.AspNetUserRoles on u.Id equals ur.UserId
                             join r in _usersRepo.AspNetRoles on ur.RoleId equals r.Id
                             where u.Id == id
                             select new UpdateUserViewModel { UserName = u.UserName, RoleName = r.Name, UserID = u.Id, RoleID = r.Id }).FirstOrDefault();
                            
            //var userData = usersQuery.ToList();

            //ViewBag.Users = userData;

            return View(usersQuery);
        }

        [HttpPost]
        public IActionResult UpdateUser(AspNetUser response)
        {
            _usersRepo.UpdateUser(response); //add record to database
            return RedirectToAction("Users");
        }

    }
}
