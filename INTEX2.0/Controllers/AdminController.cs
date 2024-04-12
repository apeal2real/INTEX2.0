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

        //public IActionResult Orders()
        //{
        //    var fraudOrders = _repo.Orders
        //        .Where(x => x.Fraud == 1)
        //        .OrderByDescending(x => x.Date)
        //        .Take(20)
        //        .ToList();
            
        //    return View(fraudOrders);
        //}
        
        public IActionResult OrderDetails(int id)
        {
            var orderToDisplay = _repo.Orders
                .Where(x => x.TransactionId == id)
                .FirstOrDefault();
            
            return View(orderToDisplay);
        }

        public IActionResult EditProduct(int id)
        {
            return View("AddProduct");
        }

        [HttpGet]
        public IActionResult UpdateUser(string id)
        {
            var usersQuery = (from u in _usersRepo.AspNetUsers
                             join ur in _usersRepo.AspNetUserRoles on u.Id equals ur.UserId
                             join r in _usersRepo.AspNetRoles on ur.RoleId equals r.Id
                             where u.Id == id
                             select new UpdateUserViewModel { UserName = u.UserName, RoleName = r.Name, UserID = u.Id, RoleID = r.Id }).FirstOrDefault();
                            
            //var userData = usersQuery.ToList();

            //ViewBag.Users = userData;

            return View(usersQuery);
        }

        [HttpPost]
        public IActionResult UpdateUser(AspNetUser response)
        {
            _usersRepo.UpdateUser(response); //add record to database
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
        public IActionResult AddEdit() //This action returns the UpdateUser page but doesn't populate it with any data
        {
            return View("UpdateUser");
        }

        [HttpPost]
        public IActionResult AddEdit(AspNetUser response) //This Post method allows the user to add a task and saves it
        {
            _usersRepo.AddUser(response); //add record to database
            return RedirectToAction("Index");
        }
    }
}
