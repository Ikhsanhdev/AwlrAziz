
using AwlrAziz;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AwlrAziz.Data;
using AwlrAziz.Models;
using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using Dapper;
using Serilog;
using Hangfire;
using Hangfire.PostgreSql;
using AwlrAziz.Interfaces;
using AwlrAziz.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson(opt =>
{
  opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});

// builder.Services.RegisterServices();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<Serilog.ILogger>(sp => Log.Logger); 
// builder.Services.AddHttpClient<ApiController>();
builder.Services.RegisterServices();
// Add services to the container.
builder.Services.AddControllersWithViews();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AwlrAzizContext>((provider, options) =>
{
  if (connectionString != null)
  {
    options.UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention()
        .UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()))
        .EnableSensitiveDataLogging();
  }
});

// #region Authentication / Authorization
// builder.Services.AddAuthorization();
// builder.Services.AddAuthentication(options =>
// {
//   options.DefaultAuthenticateScheme = "CookieAuthentication";
//   options.DefaultChallengeScheme = "CookieAuthentication";
// })
// .AddCookie("CookieAuthentication", options =>
// {
//   options.Cookie.Name = "Sih3CookiesAuth";
//   options.LoginPath = "/Auth/Login";
//   options.LogoutPath = "/Auth/Logout";
//   options.AccessDeniedPath = "/Auth/AccessDenied";
//   options.ExpireTimeSpan = TimeSpan.FromHours(6);
//   options.Cookie.Path = "/"; 
// });
// #endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  // app.UseHsts();
}

// app.UseHttpsRedirection();
// app.UseStatusCodePagesWithReExecute("/Main/PageNotFound");
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"); // <-- Update in AspnetCoreMvcStarter

app.Run();
