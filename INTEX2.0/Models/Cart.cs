namespace INTEX2._0.Models;

public class Cart
{
    public List<CartLine> Lines { get; set; } = new List<CartLine>();

    public void AddItem(Products p, int quantity)
    {
        CartLine? line = Lines
            .Where(x => x.Products.ProductIdPk == p.ProductIdPk)
            .FirstOrDefault();

        if (line == null)
        {
            Lines.Add(new CartLine{
                Products = p,
                Quantity = quantity
                });
        }
        else
        {
            line.Quantity += quantity;
        }
    }

    public void RemoveItem(Products p) => Lines.RemoveAll(x => x.Products.ProductIdPk == p.ProductIdPk);

    public void Clear() => Lines.Clear();

    public decimal CalcTotal() => (decimal)Lines.Sum(x => x.Products.Price * x.Quantity);

    public class CartLine
    {
        public int CartLineId { get; set; }
        public Products Products { get; set; }
        public int Quantity { get; set; }
    }
}