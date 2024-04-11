using System.Net;
using INTEX2._0.Infrastructure;
using INTEX2._0.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

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

    public void OnPost(int productId, int quantity)
    {
        Products prod = _repo.Products
            .FirstOrDefault(x => x.ProductId == productId);

        if (prod != null)
        {
            Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
            Cart.AddItem(prod, quantity);
            HttpContext.Session.SetJson("cart", Cart);
        }
    }
    
    public void OnPostRemoveItem(int productId)
    {
        Products productToRemove = _repo.Products.FirstOrDefault(x => x.ProductId == productId);
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
    public IActionResult OnPostNavigateToPayment()
    {
        Cart = HttpContext.Session.GetJson<Cart>("cart") ?? new Cart();
        // prep cart data for serializing
        Dictionary<int, int> cartData = new Dictionary<int, int>();
        foreach (var line in Cart.Lines)
        {
            cartData[(int)line.Products.ProductId] = line.Quantity;
        }
        // Serialize the dictionary to JSON
        string serializedData = JsonConvert.SerializeObject(cartData);
        TempData["CartData"] = serializedData;
        
        // also prep total calc data
        decimal total = Cart.CalcTotal();
        string serializedTotal = JsonConvert.SerializeObject(total);
        TempData["CartTotal"] = serializedTotal;

        Cart.Clear();
        HttpContext.Session.SetJson("cart", Cart);
        
        // Redirect to a Razor View
        return Redirect("/Home/Checkout");
    }
}