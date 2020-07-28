using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalROnlineChatServer.DataBase;
using SignalROnlineChatServer.Models;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.Hubs;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace SignalROnlineChatServer
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
            services.AddControllersWithViews();

            //services.AddMvc().AddRazorPagesOptions(options =>
            //{
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/DisplayLogin", "/Account/DisplayLogin");
            //});

            //services.AddRazorPages( options => 
            //    options.Conventions.AuthorizeAreaPage("Views", "/Login"));


            services.AddDbContext<OnlineChatDBContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.AddIdentity<User, IdentityRole>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<OnlineChatDBContext>()
                .AddDefaultTokenProviders();

            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapRazorPages();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=DisplayLogin}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}



/*
 
public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc(options => options.EnableEndpointRouting = false);

            //services.AddMvc().AddRazorPagesOptions(options =>
            //{
            //    options.Conventions.AddAreaPageRoute("Identity", "/Account/Login", "");
            //});

            //services.AddControllers()
            //    .AddJsonOptions(options =>
            //    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));        

            services.AddRazorPages();

            services.AddDbContext<OnlineChatDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.AddIdentity<User, IdentityRole>(options => {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<OnlineChatDBContext>()
                .AddDefaultTokenProviders();

            services.AddSignalR();
            //services.AddMvc();


            //services.AddControllersWithViews();

            services.AddMvc().AddRazorPagesOptions(options =>
            {
                options.Conventions.AddAreaPageRoute("Identity", "/Account/DisplayLogin", "/Account/DisplayLogin");
            });




            //services.AddDefaultIdentity<User>(options =>
            //    options.SignIn.RequireConfirmedAccount = true)
            //    .AddEntityFrameworkStores<OnlineChatDBContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            //app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseAuthentication();

            //app.UseMvcWithDefaultRoute();

            app.UseMvc(routes =>
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id}"
                ));

            //    app.UseEndpoints(endpoints =>
            //    {
            //    //endpoints.MapRazorPages();
            //    endpoints.MapControllerRoute("default", "{controller=Account}/{action=DisplayLogin}");
            //    endpoints.MapHub<ChatHub>("/chatHub");
            //    //endpoints.MapDefaultControllerRoute();
            //});

            ///

            //app.UseSignalR(routes =>
            //    routes.MapHub<ChatHub>("/chatHub"));

            //app.UseEndpoints(endpoints =>
            //    endpoints.MapHub<ChatHub>("/chatHub"));


            //app.Run(context =>
            //{
            //    context.Response.StatusCode = 404;
            //    return Task.FromResult(0);
            //});


            //app.Run( async (context) =>
            //{
            //await context.Response.WriteAsync("Hello World");
            //return Task.FromResult(0);
            //});

            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    if (context.Response.StatusCode == 404)
            //    {
            //        context.Request.Path = "/api/Index";
            //        await next();
            //    }
            //});

        }
 
 */
