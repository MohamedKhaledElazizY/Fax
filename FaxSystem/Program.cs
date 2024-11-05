using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using FaxSystem.Data;
using Microsoft.Extensions.FileProviders;
using NToastNotify;
using FaxSystem.Hubs;
var builder = WebApplication.CreateBuilder(args);
//
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Login/Login";
        option.ExpireTimeSpan = TimeSpan.FromMinutes(10);
    });
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));

//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("policy1",
//        policy =>
//        {
//            policy.WithOrigins("\\\\192.168.1.252\\directory\\Uploads\\").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
//        });

//});

builder.Services.AddRazorPages().AddNToastNotifyNoty(new NotyOptions
{
    ProgressBar = true,
    Timeout = 5000
});
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine("\\\\192.168.1.252\\Share Folder\\Uploads")),
    RequestPath = "/StaticFiles"
});


app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//       Path.Combine(@"\\192.168.1.252\directory", "Uploads")),
//    RequestPath = "/Uploads"
//});

app.UseRouting();

app.UseAuthentication();

app.UseCors();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<CHub>("/cHub");
app.UseAuthorization();
app.UseNToastNotify();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
