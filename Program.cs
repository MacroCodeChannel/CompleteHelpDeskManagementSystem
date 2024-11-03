using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Elmah;
using ElmahCore;
using ElmahCore.Mvc;
using HelpDeskSystem.ClaimsManagement;
using HelpDeskSystem.Data;
using HelpDeskSystem.Extensions;
using HelpDeskSystem.Interfaces;
using HelpDeskSystem.Models;
using HelpDeskSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using XmlFileErrorLog = ElmahCore.XmlFileErrorLog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDistributedMemoryCache();



builder.Services.AddTransient<IPdfService, PdfService>();
builder.Services.AddTransient<IExportService, ExportService>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, MyUserClaimsPrincipalFactory>()
.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.Name = ".HelpDesk.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LoginPath = "/Identity/Account/Login";
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Permission", policyBuilder =>
    {
        policyBuilder.Requirements.Add(new PermissionAuthorizationRequirement());
    });
});

builder.Services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();

var config = new MapperConfiguration(
    cfg =>
    {
        if (cfg != null)
        {
            cfg.SourceMemberNamingConvention = new PascalCaseNamingConvention();
            cfg.DestinationMemberNamingConvention = new PascalCaseNamingConvention();
            cfg.AllowNullDestinationValues = true;
            cfg.AddProfile(new AutomapperProfileService());
        }
    });
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
});

builder.Services.AddCors(option => option.AddPolicy("AllowAllOrigins", builder =>
{
    builder
    .AllowAnyOrigin()
     .AllowAnyMethod()
      .AllowAnyHeader();

}));



builder.Services.AddRouting(options => options.LowercaseUrls = true);


builder.Services.AddElmah<XmlFileErrorLog>(options =>
{
    options.Path = "ErrorLogs/errors";
    options.LogPath = builder.Configuration["FileSettings:LogsFolder"];
});



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseElmahExceptionPage();
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
app.UseCookiePolicy();
app.UseElmah();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); 
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();


