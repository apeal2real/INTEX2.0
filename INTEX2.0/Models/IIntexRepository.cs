namespace INTEX2._0.Models
{
    public interface IIntexRepository
    {
        List<Products> Products { get; }
        List<Category> Categories { get; }
        List<Customer> Customers { get; }
        List<LineItem> LineItems { get; }
        List<Order> Orders { get; }
        List<ProductsCategory> ProductsCategories { get; }
        List<Recommendation> Recommendations { get; }
        List<ProductRecommendation> ProductRecommendations { get; }

        public void AddOrder(Order order);
        public void AddLineItem(LineItem lineItem);
        public void AddCustomer(Customer customer);
    }
}
