using INTEX2._0.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;

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
            var allProducts = _repo.Products.ToList();
            ViewBag.Recommendations = _repo.ProductRecommendations.ToList();
            return View(allProducts);
        }
        public IActionResult Shop()
        {
            ViewBag.Products = _repo.Products.ToList();
            ViewBag.Categories = _repo.Categories.ToList();
            ViewBag.Colors = _repo.Products.Select(p => p.PrimaryColor).Distinct();
            return View();
        }

        [HttpPost]
        public IActionResult FilteredShop(int? categoryId, string? color, int? numItems)
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
            
            if (numItems.HasValue)
            {
                productQueryCleaned = productQueryCleaned.Take(numItems.Value);
            }
            
            var productData = productQueryCleaned.ToList();
            
            string? category = _repo.Categories
                .Where(c => c.CategoryId == categoryId)
                .Select(c => c.CategoryName)
                .FirstOrDefault();

            ViewBag.Products = productData;
            ViewBag.Category = category;
            ViewBag.Color = color;
            ViewBag.itemNum = numItems;

            return View();
        }

        public IActionResult Product(int id)
        {
            var product = _repo.Products
                .FirstOrDefault(x => x.ProductId == id);

            ViewBag.ProductRecommendations = _repo.ProductRecommendations
                .Where(x => x.ItemId == id)
                .ToList();
            ViewBag.Products = _repo.Products.ToList();

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
            string serializedData = TempData["CartData"] as string;

            // Deserialize the JSON data back to a dictionary
            Dictionary<int, int> cartData = JsonConvert.DeserializeObject<Dictionary<int, int>>(serializedData);
            ViewBag.CartDict = cartData;

            List<int> productIds = new List<int>();
            foreach (var line in cartData)
            {
                productIds.Add(line.Key);
            }

            var products = _repo.Products
                .Where(p => productIds.Contains(p.ProductId))
                .ToList();

            ViewBag.OrderNum = TempData["OrderNum"];
            ViewBag.Products = products;
            
            return View();
        }
        
        [HttpPost]
        public IActionResult SubmitLineItem(List<LineItem> lineItems)
        {
            foreach (var line in lineItems)
            {
                _repo.AddLineItem(line);
            }
            
            return RedirectToAction("Index");
        }
        
        [HttpGet]
        public IActionResult Checkout()
        {
            string serializedTotal = TempData["CartTotal"] as string;
            // Deserialize the JSON data back to a decimal
            decimal total = JsonConvert.DeserializeObject<decimal>(serializedTotal);
            int intTotal = (int)total;
            ViewBag.CartTotal = intTotal;
            
            string username = User.Identity?.Name!;
            var customer = _repo.Customers
                .Where(c => c.Email == username)
                .FirstOrDefault();

            ViewBag.cId = customer.CustomerId;
            
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            _repo.AddOrder(order);
            TempData["OrderNum"] = order.TransactionId;
            return RedirectToAction("OrderConfirm");
        }
    }
}
