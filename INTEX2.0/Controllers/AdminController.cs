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
                             select new UpdateUserViewModel
                             {
                                 UserId = u.Id,
                                 UserName = u.UserName,
                                 NormalizedUserName = u.NormalizedUserName,
                                 Email = u.Email,
                                 NormalizedEmail = u.NormalizedEmail,
                                 PasswordHash = u.PasswordHash,
                                 SecurityStamp = u.SecurityStamp,
                                 ConcurrencyStamp= u.ConcurrencyStamp,
                                 PhoneNumber = u.PhoneNumber,
                                 PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                                 TwoFactorEnabled = u.TwoFactorEnabled,
                                 LockoutEnd = u.LockoutEnd,
                                 LockoutEnabled = u.LockoutEnabled,
                                 AccessFailedCount = u.AccessFailedCount,
                                 RoleName = r.Name,
                                 RoleID = ur.RoleId,
                             }).FirstOrDefault();
                            
            //var userData = usersQuery.ToList();

            //ViewBag.Users = userData;

            return View(usersQuery);
        }

        [HttpPost]
        public IActionResult UpdateUser(UpdateUserViewModel response)
        {
            AspNetUser user = _usersRepo.AspNetUsers
                .FirstOrDefault(u => u.Id == response.UserId);
            user.Id = response.UserId;
            user.UserName = response.UserName;
            // user.NormalizedUserName = response.NormalizedUserName;
            // user.Email = response.Email;
            // user.NormalizedEmail = response.NormalizedEmail;
            // user.EmailConfirmed = response.EmailConfirmed;
            // user.PasswordHash = response.PasswordHash;
            // user.SecurityStamp = response.SecurityStamp;
            // user.ConcurrencyStamp = response.ConcurrencyStamp;
            // user.PhoneNumber = response.PhoneNumber;
            // user.PhoneNumberConfirmed = response.PhoneNumberConfirmed;
            // user.TwoFactorEnabled = response.TwoFactorEnabled;
            // user.AccessFailedCount = response.AccessFailedCount;
            
            _usersRepo.UpdateUser(user); //add record to database
            
            // update user role table
            AspNetUserRole userRole = _usersRepo.AspNetUserRoles
                .FirstOrDefault(ur => ur.UserId == response.UserId);
            
            userRole.RoleId = response.RoleID;
            userRole.UserId = response.UserId;

            _usersRepo.UpdateUserRole(userRole);
            
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            UpdateUserViewModel user = new UpdateUserViewModel();
            return View(user);
        }

        [HttpPost]
        public IActionResult AddUser(UpdateUserViewModel response)
        {
            AspNetUser user = new AspNetUser();
            
            // hash the email and call it the Id
            string id = response.ComputeSha256Hash(response.Email);
            user.Id = id;
            user.UserName = response.Email;
            // user.NormalizedUserName = response.UserName.ToUpper();
            user.Email = response.Email;
            // user.NormalizedEmail = response.Email.ToUpper();
            
            // hashes user inputted password
            var hashedPassword = response.ComputeSha256Hash(response.PasswordHash);
            user.PasswordHash = hashedPassword;
            user.PhoneNumber = response.PhoneNumber;
            
            _usersRepo.AddUser(user); //add record to database
            
            // update user role table
            AspNetUserRole userRole = new AspNetUserRole();
            userRole.RoleId = response.RoleID;
            userRole.UserId = id;

            _usersRepo.AddUserRole(userRole);
            
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult RemoveUser(string id) //This Get method retrieves a confirmation page for the deletion of a record
        {
            // ViewBag.Tasks = _repo.Tasks.ToList();
            var recordToDelete = _usersRepo.AspNetUsers
                .Single(x => x.Id == id);

            return View("Confirmation", recordToDelete);
        }

        [HttpPost]
        public IActionResult RemoveUser(AspNetUser deleted) //This Post Method deletes a task from the database
        {
            _usersRepo.RemoveUser(deleted);
            return RedirectToAction("Users");
        }

        // [HttpGet]
        // public IActionResult AddEdit() //This action returns the UpdateUser page but doesn't populate it with any data
        // {
        //     return View("UpdateUser");
        // }
        //
        // [HttpPost]
        // public IActionResult AddEdit(AspNetUser response) //This Post method allows the user to add a task and saves it
        // {
        //     _usersRepo.AddUser(response); //add record to database
        //     return RedirectToAction("Index");
        // }
    }
}
