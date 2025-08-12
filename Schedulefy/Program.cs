using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Schedulefy.Data;
using Schedulefy.Data.Models;
using Schedulefy.Services.Core;
using Schedulefy.Services.Core.Contracts;

namespace Schedulefy
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            builder.Services.AddDbContext<SchedulefyDbContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddControllersWithViews();

            builder.Services.AddDefaultIdentity<IdentityUser>(options => 
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
            })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<SchedulefyDbContext>();

            builder.Services.AddScoped<IEventService, EventService>();
            builder.Services.AddScoped<ICategoryService, CategoryService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<ICommentService, CommentService>();

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

            app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            app.MapRazorPages();

            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                string[] roleName = { "Admin", "User" };

                foreach (var role in roleName)
                {
                    var roleExist = await roleManager.RoleExistsAsync(role);

                    if (!roleExist)
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }

                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                var user = await userManager.FindByEmailAsync("admin@abv.bg");

                if (user is not null && !(await userManager.IsInRoleAsync(user, "Admin")))
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }

            app.Run();

           
        }
    }
}
