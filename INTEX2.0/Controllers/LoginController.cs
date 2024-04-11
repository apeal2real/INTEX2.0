namespace INTEX2._0.Models;
using Microsoft.AspNetCore.Mvc;

public class LoginController : Controller
{
    private IIntexRepository _repo;

    public LoginController(IIntexRepository temp)
    {
        _repo = temp;
    }
    public IActionResult Login()
    {
        return View();
    }
}