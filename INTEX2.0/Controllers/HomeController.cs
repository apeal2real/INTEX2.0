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

        [HttpPost]
        public IActionResult FilteredShop(int? categoryId, string? color)
        {
            var productsQuery = from p in _repo.Products
                join pc in _repo.ProductsCategories on p.ProductId equals pc.ProductId
                join c in _repo.Categories on pc.CategoryId equals c.CategoryId
                select new { Product = p, Category = c };

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(x => x.Category.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(color))
            {
                productsQuery = productsQuery.Where(x => x.Product.PrimaryColor == color || x.Product.SecondaryColor == color);
            }

            var productQueryCleaned = productsQuery.Select(x => x.Product).DistinctBy(x => x.ProductId);
            var productData = productQueryCleaned.ToList();
            
            string? category = _repo.Categories
                .Where(c => c.CategoryId == categoryId)
                .Select(c => c.CategoryName)
                .FirstOrDefault();

            ViewBag.Products = productData;
            ViewBag.Category = category;
            ViewBag.Color = color;

            return View();
        }

        public IActionResult Product(int id)
        {
            var product = _repo.Products
                .FirstOrDefault(x => x.ProductId == id);
            
            return View(product);
        }
        
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
