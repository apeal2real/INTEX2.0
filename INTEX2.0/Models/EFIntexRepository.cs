
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
    }
}
