using System.Net;
using INTEX2._0.Infrastructure;
using INTEX2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace INTEX2._0.Pages;

public class CartModel : PageModel
{
    public Cart? Cart { get; set; }

    private IIntexRepository _repo;

    public CartModel(IIntexRepository temp)
    {
        _repo = temp;
    }
    
    public void OnGet()
    {
        Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
    }

    public void OnPost(int productId)
    {
        Products prod = _repo.Products
            .FirstOrDefault(x => x.ProductIdPk == productId);

        if (prod != null)
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Cart.AddItem(prod, 1);
            HttpContext.Session.SetJson("cart", Cart);
        }
    }
    
    public void OnPostRemoveItem(int productId)
    {
        Products productToRemove = _repo.Products.FirstOrDefault(x => x.ProductIdPk == productId);
        if (productToRemove != null)
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Cart.RemoveItem(productToRemove);
            HttpContext.Session.SetJson("cart", Cart);
        }
    }
    public void OnPostClearCart()
    {
        Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        Cart.Clear();
        HttpContext.Session.SetJson("cart", Cart);
    }
    public IActionResult OnPostNavigateToOrderConfirm()
    {
        Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        Cart.Clear();
        HttpContext.Session.SetJson("cart", Cart);
        
        // Redirect to a Razor View
        return Redirect("/Home/OrderConfirm");
    }
}