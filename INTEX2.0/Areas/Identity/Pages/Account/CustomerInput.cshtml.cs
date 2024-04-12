// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using INTEX2._0.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace INTEX2._0.Areas.Identity.Pages.Account
{
    public sealed class CustomerModel : PageModel
    {
        [BindProperty]
        public Customer Customer { get; set; }

        public IActionResult OnGet()
        {
            // Create a new instance of Customer
            Customer = new Customer();
            
            return Page();
        }

        public IActionResult OnPost(Customer customer, [FromServices] IIntexRepository repo)
        {
            Customer = customer;

            repo.AddCustomer(customer);
            
            // Serialize the Customer object to JSON
            string serializedCustomer = JsonConvert.SerializeObject(Customer);
            TempData["SerializedCustomer"] = serializedCustomer;
            
            return RedirectToPage("Register");
        }
    }
}
