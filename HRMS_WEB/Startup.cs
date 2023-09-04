using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using HRMS_WEB.Models;
using HRMS_WEB.DbContext;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using DevExpress.AspNetCore;
using DevExpress.XtraReports.Web.Extensions;
using HRMS_WEB.Services;

namespace HRMS_WEB
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
            services.AddDevExpressControls();
            services.AddControllersWithViews();

            var emailConfig = Configuration
                .GetSection("EmailConfiguration")
                .Get<EmailConfiguration>();

            services.AddSingleton(emailConfig);

            // MySQL DB connection service
            services.AddDbContextPool<HRMSDbContext>(options =>
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            //// EFCore identity and set password validations  
            //services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            //{
            //    options.Password.RequiredLength = 6;
            //    options.Password.RequireDigit = false;
            //    options.Password.RequireUppercase = false;
            //    options.Password.RequiredUniqueChars = 0;
            //    options.Password.RequireNonAlphanumeric = false;
            //}).AddEntityFrameworkStores<HRMSDbContext>();

            // Automapper service for DTO's
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Repository dependancy injection
            services.AddScoped<ReportStorageWebExtension, ReportStorageWebExtension1>();

            //if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //{
            //    DevExpress.Printing.CrossPlatform.CustomEngineHelper.RegisterCustomDrawingEngine(
            //        typeof(DevExpress.CrossPlatform.Printing.DrawingEngine.PangoCrossPlatformEngine));
            //}
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.DataBindingMode = DevExpress.XtraReports.UI.DataBindingMode.Expressions;
            app.UseDevExpressControls();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "node_modules")),

                RequestPath = "/node_modules"
            });

            app.UseStaticFiles();

            //app.UseAuthentication();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}