using INTEX2._0.Models;
using INTEX2._0.Models.ViewModels;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System.Drawing;
using X.PagedList;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IWebHostEnvironment;

namespace INTEX2._0.Controllers
{
    public class AdminController : Controller
    {
        private IIntexRepository _repo;
        private IUsers _usersRepo; // Add IUsers dependency
        private readonly InferenceSession _session;
        private readonly string _onnxModelPath;

        public AdminController(IIntexRepository temp, IUsers usersRepo, IHostingEnvironment hostEnvironment) // Include IUsers in the constructor
        {
            _repo = temp;
            _usersRepo = usersRepo; // Assign IUsers dependency
            _onnxModelPath = System.IO.Path.Combine(hostEnvironment.ContentRootPath, "wwwroot", "fraudModel.onnx");
            _session = new InferenceSession(_onnxModelPath);
        }

        public IActionResult Orders()
        {

            var records = _repo.Orders
            .Select(o => new
            {
                Order = o,
                ParsedDate = DateTime.Parse(o.Date)
            })
            .OrderByDescending(o => o.ParsedDate)
            .Take(20)
            .Select(o => o.Order) // Select back the original order objects
            .ToList(); // Fetch the 20 most recent records

            var predictions = new List<OrderPrediction>();

            // Dictionary mapping the numeric prediction to an animal type
            var class_type_dict = new Dictionary<int, string>
                {
                    { 0, "Not Fraud" },
                    { 1, "Fraud" }
                };

            foreach (var record in records)
            {
                // Calculate days since January 1, 2022
                var january1_2022 = new DateTime(2022, 1, 1);

                DateTime? newDate = DateTime.Parse(record.Date);

                var daysSinceJan12022 = newDate.HasValue ? Math.Abs((newDate.Value - january1_2022).Days) : 0;

                //var daysSinceJan12022 = 0; // Default value
                //if (!string.IsNullOrEmpty(record.Date))
                //{
                //    DateTime dateValue;
                //    if (DateTime.TryParse(record.Date, out dateValue))
                //    {
                //        daysSinceJan12022 = Math.Abs((dateValue - january1_2022).Days);
                //    }
                //}

                // Preprocess features to make them compatible with the model
                var input = new List<float>
                {
                    (float)record.TransactionId,
                    (float)(record.CustomerId ?? 0),
                    (float)(record.Time ?? 0),    
                    // fix amount if it's null
                    (float)(record.Amount ?? 0),
                    // fix date
                    daysSinceJan12022,
                    // Check the Dummy Coded Data
                    record.DayOfWeek == "Mon" ? 1 : 0,
                    record.DayOfWeek == "Sat" ? 1 : 0,
                    record.DayOfWeek == "Sun" ? 1 : 0,
                    record.DayOfWeek == "Thu" ? 1 : 0,
                    record.DayOfWeek == "Tue" ? 1 : 0,
                    record.DayOfWeek == "Wed" ? 1 : 0,
                    record.EntryMode == "Pin" ? 1 : 0,
                    record.EntryMode == "Tap" ? 1 : 0,
                    record.TypeOfTransaction == "Online" ? 1 : 0,
                    record.TypeOfTransaction == "POS" ? 1 : 0,
                    record.CountryOfTransaction == "India" ? 1 : 0,
                    record.CountryOfTransaction == "Russia" ? 1 : 0,
                    record.CountryOfTransaction == "USA" ? 1 : 0,
                    record.CountryOfTransaction == "UnitedKingdom" ? 1 : 0,
                    // Use CountryOfTransaction if ShippingAddress is null
                    (record.ShippingAddress ?? record.CountryOfTransaction) == "India" ? 1 : 0,
                    (record.ShippingAddress ?? record.CountryOfTransaction) == "Russia" ? 1 : 0,
                    (record.ShippingAddress ?? record.CountryOfTransaction) == "USA" ? 1 : 0,
                    (record.ShippingAddress ?? record.CountryOfTransaction) == "UnitedKingdom" ? 1 : 0,
                    record.Bank == "HSBC" ? 1 : 0,
                    record.Bank == "Halifax" ? 1 : 0,
                    record.Bank == "Lloyds" ? 1 : 0,
                    record.Bank == "Metro" ? 1 : 0,
                    record.Bank == "Monzo" ? 1 : 0,
                    record.Bank == "RBS" ? 1 : 0,
                    record.TypeOfCard == "Visa" ? 1 : 0
                };
                var inputTensor = new DenseTensor<float>(input.ToArray(), new[] { 1, input.Count });
                var inputs = new List<NamedOnnxValue>
                {
                    NamedOnnxValue.CreateFromTensor("float_input", inputTensor)
                };
                string predictionResult;
                using (var results = _session.Run(inputs))
                {
                    var prediction = results.FirstOrDefault(item => item.Name == "output_label")?.AsTensor<long>().ToArray();
                    predictionResult = prediction != null && prediction.Length > 0 ? class_type_dict.GetValueOrDefault((int)prediction[0], "Unknown") : "Error in prediction";
                }
                predictions.Add(new OrderPrediction { Orders = record, Prediction = predictionResult }); // Adds the animal information and prediction for that animal to AnimalPrediction viewmodel
            }

            return View(predictions);
        }
        public IActionResult Users()
        {
            // Fetch users from the database along with their roles
            var usersQuery = from u in _usersRepo.AspNetUsers
                             join ur in _usersRepo.AspNetUserRoles on u.Id equals ur.UserId
                             join r in _usersRepo.AspNetRoles on ur.RoleId equals r.Id
                             select new { u.UserName, r.Name, UserID = u.Id, RoleID = r.Id };

            var userData = usersQuery.ToList();

            ViewBag.Users = userData;

            return View();
        }

        public IActionResult Products()
        {
            ViewBag.Products = _repo.Products.ToList();
            ViewBag.Categories = _repo.Categories.ToList();
            ViewBag.Colors = _repo.Products.Select(p => p.PrimaryColor).Distinct();
            return View();
        }
        
        public IActionResult OrderDetails(int id)
        {
            var orderToDisplay = _repo.Orders
                .Where(x => x.TransactionId == id)
                .FirstOrDefault();
            
            return View(orderToDisplay);
        }

        [HttpPost]
        public IActionResult OrderDetails(Order orderToDelete)
        {
            _repo.RemoveOrder(orderToDelete);
            return RedirectToAction("Orders");
        }

        [HttpGet]
        public IActionResult UpdateUser(string id)
        {
            var usersQuery = (from u in _usersRepo.AspNetUsers
                             join ur in _usersRepo.AspNetUserRoles on u.Id equals ur.UserId
                             join r in _usersRepo.AspNetRoles on ur.RoleId equals r.Id
                             where u.Id == id
                             select new UpdateUserViewModel
                             {
                                 UserId = u.Id,
                                 UserName = u.UserName,
                                 NormalizedUserName = u.NormalizedUserName,
                                 Email = u.Email,
                                 NormalizedEmail = u.NormalizedEmail,
                                 PasswordHash = u.PasswordHash,
                                 SecurityStamp = u.SecurityStamp,
                                 ConcurrencyStamp= u.ConcurrencyStamp,
                                 PhoneNumber = u.PhoneNumber,
                                 PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                                 TwoFactorEnabled = u.TwoFactorEnabled,
                                 LockoutEnd = u.LockoutEnd,
                                 LockoutEnabled = u.LockoutEnabled,
                                 AccessFailedCount = u.AccessFailedCount,
                                 RoleName = r.Name,
                                 RoleID = ur.RoleId,
                             }).FirstOrDefault();

            return View(usersQuery);
        }

        [HttpPost]
        public IActionResult UpdateUser(UpdateUserViewModel response)
        {
            AspNetUser user = _usersRepo.AspNetUsers
                .FirstOrDefault(u => u.Id == response.UserId);
            user.Id = response.UserId;
            user.UserName = response.UserName;
            
            _usersRepo.UpdateUser(user); //add record to database
            
            // update user role table
            AspNetUserRole userRole = _usersRepo.AspNetUserRoles
                .FirstOrDefault(ur => ur.UserId == response.UserId);
            
            userRole.RoleId = response.RoleID;
            userRole.UserId = response.UserId;

            _usersRepo.UpdateUserRole(userRole);
            
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult AddUser()
        {
            UpdateUserViewModel user = new UpdateUserViewModel();
            return View(user);
        }

        [HttpPost]
        public IActionResult AddUser(UpdateUserViewModel response)
        {
            AspNetUser user = new AspNetUser();
            
            // hash the email and call it the Id
            string id = response.ComputeSha256Hash(response.Email);
            user.Id = id;
            user.UserName = response.Email;
            // user.NormalizedUserName = response.UserName.ToUpper();
            user.Email = response.Email;
            // user.NormalizedEmail = response.Email.ToUpper();
            
            // hashes user inputted password
            var hashedPassword = response.ComputeSha256Hash(response.PasswordHash);
            user.PasswordHash = hashedPassword;
            user.PhoneNumber = response.PhoneNumber;
            
            _usersRepo.AddUser(user); //add record to database
            
            // update user role table
            AspNetUserRole userRole = new AspNetUserRole();
            userRole.RoleId = response.RoleID;
            userRole.UserId = id;

            _usersRepo.AddUserRole(userRole);
            
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult RemoveUser(string id) //This Get method retrieves a confirmation page for the deletion of a record
        {
            // ViewBag.Tasks = _repo.Tasks.ToList();
            var recordToDelete = _usersRepo.AspNetUsers
                .Single(x => x.Id == id);

            return View("Confirmation", recordToDelete);
        }

        [HttpPost]
        public IActionResult RemoveUser(AspNetUser deleted) //This Post Method deletes a task from the database
        {
            _usersRepo.RemoveUser(deleted);
            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            PCViewModel newProd = new PCViewModel();

            var categories = _repo.Categories
                .Select(c => c.CategoryName)
                .Distinct();

            ViewBag.Categories = categories;
            
            return View(newProd);
        }
        
        [HttpPost]
        public IActionResult AddProduct(PCViewModel response)
        {
            Products product = new Products();
            product.Name = response.Name;
            product.Year = response.Year;
            product.NumParts = response.NumParts;
            product.Price = response.Price;
            product.ImgLink = response.ImgLink;
            product.PrimaryColor = response.PrimaryColor;
            product.SecondaryColor = response.SecondaryColor;
            product.ShortDescription = response.ShortDescription;
            product.Description = response.Description;

            var latestID = _repo.Products
                .OrderByDescending(p => p.ProductId)
                .FirstOrDefault()?.ProductId;

            product.ProductId = (int)(latestID + 1);
            

            _repo.AddProduct(product);

            var categoryId = _repo.Categories
                .Where(c => c.CategoryName == response.CategoryName)
                .Select(c => c.CategoryId)
                .FirstOrDefault();

            ProductsCategory prodCat = new ProductsCategory();
            prodCat.ProductId = product.ProductId;
            prodCat.CategoryId = categoryId;
            _repo.AddProductCategory(prodCat);
            
            return RedirectToAction("Products");
        }
        
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var product = (from p in _repo.Products
                join pc in _repo.ProductsCategories on p.ProductId equals pc.ProductId
                join c in _repo.Categories on pc.CategoryId equals c.CategoryId
                where p.ProductId == id
                select new PCViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Year = p.Year,
                    NumParts = p.NumParts,
                    Price = p.Price,
                    ImgLink = p.ImgLink,
                    PrimaryColor = p.PrimaryColor,
                    SecondaryColor = p.SecondaryColor,
                    Description = p.Description,
                    CategoryName = c.CategoryName
                }).FirstOrDefault();
            
            var categories = _repo.Categories
                .Select(c => c.CategoryName)
                .Distinct();

            ViewBag.Categories = categories;
            
            return View(product);
        }
        
        [HttpPost]
        public IActionResult EditProduct(PCViewModel response)
        {
            Products product = _repo.Products
                .Where(p => p.ProductId == response.ProductId)
                .FirstOrDefault();
            
            product.Name = response.Name;
            product.Year = response.Year;
            product.NumParts = response.NumParts;
            product.Price = response.Price;
            product.ImgLink = response.ImgLink;
            product.PrimaryColor = response.PrimaryColor;
            product.SecondaryColor = response.SecondaryColor;
            product.ShortDescription = response.ShortDescription;
            product.Description = response.Description;

            _repo.EditProduct(product);

            var categoryId = _repo.Categories
                .Where(c => c.CategoryName == response.CategoryName)
                .Select(c => c.CategoryId)
                .FirstOrDefault();

            ProductsCategory prodCat = _repo.ProductsCategories
                .Where(p => p.ProductId == response.ProductId)
                .FirstOrDefault();
            
            prodCat.ProductId = product.ProductId;
            prodCat.CategoryId = categoryId;
            _repo.EditProductCategory(prodCat);
            
            return RedirectToAction("Products");
        }
        
        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            var product = (from p in _repo.Products
                join pc in _repo.ProductsCategories on p.ProductId equals pc.ProductId
                join c in _repo.Categories on pc.CategoryId equals c.CategoryId
                where p.ProductId == id
                select new PCViewModel
                {
                    ProductId = p.ProductId,
                    Name = p.Name,
                    Year = p.Year,
                    NumParts = p.NumParts,
                    Price = p.Price,
                    ImgLink = p.ImgLink,
                    PrimaryColor = p.PrimaryColor,
                    SecondaryColor = p.SecondaryColor,
                    Description = p.Description,
                    CategoryName = c.CategoryName
                }).FirstOrDefault();
            
            return View(product);
        }
        
        [HttpPost]
        public IActionResult DeleteProductPost(int id)
        {
            Products product = _repo.Products
                .Where(p => p.ProductId == id)
                .FirstOrDefault();
           

            ProductsCategory prodCat = _repo.ProductsCategories
                .Where(p => p.ProductId == id)
                .FirstOrDefault();

            _repo.DeleteProductCategory(prodCat);
            _repo.DeleteProduct(product);

            return RedirectToAction("Products");
        }
    }
}
