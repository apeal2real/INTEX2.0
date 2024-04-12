using INTEX2._0.Data;
using INTEX2._0.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.ML.OnnxRuntime;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var services = builder.Services;
        var configuration = builder.Configuration;

        // // Create a new SecretClient to retrieve secrets from Azure Key Vault
        // var keyVaultUri = new Uri("https://brickblockssecrets.vault.azure.net/");
        // var credential = new DefaultAzureCredential();
        // var secretClient = new SecretClient(vaultUri: keyVaultUri, credential: credential);
        //
        // // Retrieve secrets from Azure Key Vault
        // var clientId = (await secretClient.GetSecretAsync("GoogleClientID")).Value.Value;
        // var clientSecret = (await secretClient.GetSecretAsync("GoogleClientSecret")).Value.Value;
        //
        // //services.AddAuthentication().AddMicrosoftAccount(microsoftOptions =>
        // //{
        // //    microsoftOptions.ClientId = configuration["Authentication:Microsoft:ClientId"];
        // //    microsoftOptions.ClientSecret = configuration["Authentication:Microsoft:ClientSecret"];
        // //});
        //
        // services.AddAuthentication().AddGoogle(googleOptions =>
        // {
        //     googleOptions.ClientId = clientId /*configuration["Authentication:Google:ClientId2"]*/;
        //     googleOptions.ClientSecret = clientSecret /*configuration["Authentication:Google:ClientSecret2"]*/;
        // });

        // Add services to the container.
        var connectionString2 = builder.Configuration.GetConnectionString("MyDatabaseConnection");
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDbContext<IntexContext>(options =>
            options.UseSqlite(connectionString2));
        services.AddDbContext<MfalabW24Context>(options =>
            options.UseSqlite(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        builder.Services.AddControllersWithViews();

        builder.Services.AddScoped<IIntexRepository, EFIntexRepository>();
        builder.Services.AddScoped<IUsers, EFUsers>();

        //services.AddSingleton<InferenceSession>(
        //    new InferenceSession(".\\fraudModel.onnx")
        //);

        //services.AddSingleton<InferenceSession>(provider =>
        //{
        //    // Provide the path to the ONNX model file
        //    string modelPath = ".\\fraudModel.onnx";
        //    return new InferenceSession(modelPath);
        //});

        builder.Services.AddRazorPages();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseSession();

        app.UseRouting();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        app.MapRazorPages();

        app.UseAuthentication();
        app.UseAuthorization();

        using (var scope = app.Services.CreateScope())
        {
            var roleManager =
                scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager =
                scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string email = "admin@admin.com";
            string password = "Test1234!";

            string email2 = "william.turner@gmail.com";
            string password2 = "Willturn1234!";

            string email3 = "Maryella@gmail.com";
            string password3 = "Maryella1234!";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser();
                user.UserName = email;
                user.Email = email;
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, password);

                await userManager.AddToRoleAsync(user, "Admin");
            }

            if (await userManager.FindByEmailAsync(email2) == null)
            {
                var user = new IdentityUser();
                user.UserName = email2;
                user.Email = email2;
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, password2);

                await userManager.AddToRoleAsync(user, "User");
            }


            if (await userManager.FindByEmailAsync(email3) == null)
            {
                var user = new IdentityUser();
                user.UserName = email3;
                user.Email = email3;
                user.EmailConfirmed = true;

                await userManager.CreateAsync(user, password3);

                await userManager.AddToRoleAsync(user, "User");
            }
        }


        app.Run();
    }
}


