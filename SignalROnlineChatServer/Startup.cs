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
using Microsoft.AspNetCore.Authentication.Cookies;
using SignalROnlineChatServer.Controllers;
using SignalROnlineChatServer.BLL.Services;
using AutoMapper;
using SignalROnlineChatServer.BLL.Mapper;
using SignalROnlineChatServer.BLL.Services.Interfaces;

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

            services.AddAutoMapper(typeof(Startup));

            services.AddDbContext<OnlineChatDBContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("Default")));

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            })
                .AddEntityFrameworkStores<OnlineChatDBContext>()
                .AddDefaultTokenProviders();


            services.AddSignalR(hubOptions =>
                hubOptions.KeepAliveInterval = System.TimeSpan.FromMinutes(1));

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                options.LoginPath = "/Identity/Account/Login";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddCors(options => options.AddPolicy("CorsPolicy",
            builder =>
            {
                builder.AllowAnyMethod().AllowAnyHeader()
                       .WithOrigins("http://localhost:44318")  // 55830
                       .AllowCredentials();
            }));

            
            services.AddHttpContextAccessor();
            services.AddScoped<IHomeService, HomeService>();
            services.AddTransient<HomeService>();
            services.AddScoped<IChatService, ChatService>();
            services.AddTransient<ChatService>();
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

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=DisplayLogin}/{id?}");
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<HomeHub>("/homeHub");
            });

            


        }
    }
}
