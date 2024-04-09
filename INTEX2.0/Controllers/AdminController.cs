﻿using INTEX2._0.Models;
using Microsoft.AspNetCore.Mvc;

namespace INTEX2._0.Controllers
{
    public class AdminController : Controller
    {
        private IIntexRepository _repo;

        public AdminController(IIntexRepository temp)
        {
            _repo = temp;
        }

        public IActionResult Users()
        {
            return View();
        }
        public IActionResult Products()
        {
            ViewBag.Products = _repo.Products.ToList();

            return View();
        }
        public IActionResult Orders()
        {
            return View();
        }
    }
}
