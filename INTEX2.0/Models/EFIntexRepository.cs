
using SQLitePCL;
using System.Drawing.Text;

namespace INTEX2._0.Models
{
    public class EFIntexRepository : IIntexRepository
    {
        private IntexContext _context;
        public EFIntexRepository(IntexContext temp) 
        {
            _context = temp;
        }
        public List<Products> Products => _context.Products.ToList();
        public List<Category> Categories => _context.Categories.ToList();
        public List<Customer> Customers => _context.Customers.ToList();
        public List<LineItem> LineItems => _context.LineItems.ToList();
        public List<Order> Orders => _context.Orders.ToList();
        public List<ProductsCategory> ProductsCategories => _context.ProductsCategories.ToList();
        public List<Recommendation> Recommendations => _context.Recommendations.ToList();
        public List<ProductRecommendation> ProductRecommendations => _context.ProductRecommendations.ToList();
        
        public void AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();
        }
        
        public void AddLineItem(LineItem lineItem)
        {
            _context.LineItems.Add(lineItem);
            _context.SaveChanges();
        }

        public void AddCustomer(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
        }

        public void AddProduct(Products product)
        {
            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public void EditProduct(Products product)
        {
            _context.Products.Update(product);
            _context.SaveChanges();
        }

        public void DeleteProduct(Products product)
        {
            _context.Products.Remove(product);
            _context.SaveChanges();
        }

        public void AddProductCategory(ProductsCategory prodCat)
        {
            _context.ProductsCategories.Add(prodCat);
            _context.SaveChanges();
        }

        public void EditProductCategory(ProductsCategory prodCat)
        {
            _context.ProductsCategories.Update(prodCat);
            _context.SaveChanges();
        }

        public void DeleteProductCategory(ProductsCategory prodCat)
        {
            _context.ProductsCategories.Remove(prodCat);
            _context.SaveChanges();
        }
    }
}
