using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Interactive_Internship_Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Interactive_Internship_Application.Models;

namespace Interactive_Internship_Application
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //below statement is added to access the appsettings.json file for email purposes
        //    services.Configure<Global.AppSettings>(Configuration.GetSection("Email"));

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });    

            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(Configuration.GetConnectionString("LocalServer")));

            services.AddDefaultIdentity<IdentityUser>(options =>
            {
                // Default Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan =
                TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // Default Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;
            })
            .AddDefaultUI(UIFramework.Bootstrap4)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            CreateUserRoles(services).Wait();
        }


        private async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            IdentityResult roleResult;
            //Below is dummy data to create roles mimicking the functionality of our application. Winthrop should fix this with their authentication system upon installation
            
            //Adding Admin Role 
            var roleCheckAdmin = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheckAdmin)
            {
                //create the roles and seed them to the database 
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin"));
            }

            //Adding Student Role 
            var roleCheckStudent = await RoleManager.RoleExistsAsync("Student");
            if (!roleCheckStudent)
            {
                //create the roles and seed them to the database 
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Student"));
            }

            //Adding Professor Role 
            var roleCheckProfessor = await RoleManager.RoleExistsAsync("Professor");
            if (!roleCheckProfessor)
            {
                //create the roles and seed them to the database 
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Professor"));
            }

            //Adding Dept Role 
            var roleCheckDept = await RoleManager.RoleExistsAsync("Dept");
            if (!roleCheckDept)
            {
                //create the roles and seed them to the database 
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Dept"));
            }
/*
            //Adding Employer Role 
            var roleCheckEmployer = await RoleManager.RoleExistsAsync("Employer");
            if (!roleCheckEmployer)
            {
                //create the roles and seed them to the database 
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Employer"));
            }
*/
            //the below gives people roles, for testing and should be changed with winthrop authentication as well
            IdentityUser userStudent = await UserManager.FindByEmailAsync("milokjovicm2@mailbox.winthrop.edu");
            var UserStudent = new IdentityUser();
            await UserManager.AddToRoleAsync(userStudent, "Student");

            IdentityUser userProfessor = await UserManager.FindByEmailAsync("branhami2@mailbox.winthrop.edu");
            var UserProfessor = new IdentityUser();
            await UserManager.AddToRoleAsync(userProfessor, "Professor");

            IdentityUser userDept = await UserManager.FindByEmailAsync("granhamd12@mailbox.winthrop.edu");
            var UserDept = new IdentityUser();
            await UserManager.AddToRoleAsync(userDept, "Dept");

 /*           IdentityUser userEmployer = await UserManager.FindByEmailAsync("lloydb2@mailbox.winthrop.edu");
            var UserEmployer = new IdentityUser();
            await UserManager.AddToRoleAsync(userEmployer, "Employer");

            IdentityUser userEmployer2 = await UserManager.FindByEmailAsync("brandonadill1@gmail.com");
            var UserEmployer2 = new IdentityUser();
            await UserManager.AddToRoleAsync(userEmployer2, "Employer");
*/

            //Assign Admin role to the main User here we have given our newly registered  
            //login id for Admin management 
            IdentityUser userAdmin = await UserManager.FindByEmailAsync("dillb3@mailbox.winthrop.edu");
            var UserAdmin = new IdentityUser();
            await UserManager.AddToRoleAsync(userAdmin, "Admin");
        }
    }
}
