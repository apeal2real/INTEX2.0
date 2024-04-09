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
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Shop()
        {
            return View();
        }

        public IActionResult Product()
        {
            return View();
        }
        //[Authorize(Roles ="User")]
        public IActionResult Cart() 
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secrets()
        {
            return View();
        }
    }
}
