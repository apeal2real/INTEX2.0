using INTEX2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace INTEX2._0.Controllers
{
    public class HomeController : Controller
    {
        private IIntexRepository _repo;

        public HomeController(IIntexRepository temp)
        {
            _repo = temp;
        }

        public IActionResult Index()
        {
            Cart cart = new Cart();
            ViewBag.Cart = cart;
            
            var products = _repo.Products.ToList();
            return View(products);
        }
        public IActionResult Shop()
        {
            ViewBag.Products = _repo.Products.ToList();
            ViewBag.Categories = _repo.Categories.ToList();
            ViewBag.Colors = _repo.Products.Select(p => p.PrimaryColor).Distinct();
            return View();
        }

        public IActionResult Product(int id)
        {
            var product = _repo.Products
                .FirstOrDefault(x => x.ProductId == id);
            
            return View(product);
        }
        //[Authorize(Roles ="User")]
        // public IActionResult Cart() 
        // {
        //     return View();
        // }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult OrderConfirm()
        {
            return View();
        }
    }
}
