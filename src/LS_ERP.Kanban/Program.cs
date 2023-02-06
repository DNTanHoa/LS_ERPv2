using Logic.Config;
using LS_ERP.Kanban.Logic;
using LS_ERP.Ultilities;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Ultils.Config;
using Ultils.Helpers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddHttpClient();
builder.Services
    .Configure<BackEndConfig>(builder.Configuration.GetSection(BackEndConfig.ConfigName));
//
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/authenticate/login";
    });
//
builder.Services.AddProgressiveWebApp();
//
builder.Services
    .Configure<AuthenticateConfig>(builder.Configuration.GetSection(AuthenticateConfig.ConfigName));
builder.Services
    .Configure<ErrorConfig>(builder.Configuration.GetSection(ErrorConfig.ConfigName));

builder.Services
    .Configure<StaticFileConfig>(builder.Configuration.GetSection(StaticFileConfig.ConfigName));
builder.Services
    .Configure<NoteConfig>(builder.Configuration.GetSection(NoteConfig.ConfigName));
builder.Services
    .Configure<GroupConfig>(builder.Configuration.GetSection(GroupConfig.ConfigName));
builder.Services
    .AddSingleton<HtmlEncoder>(HtmlEncoder.Create(allowedRanges: new[] { UnicodeRanges.All }));

builder.Services.AddScoped<IAuthenticateHelper, AuthenticateHelper>();
//
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    //pattern: "{controller=Dashboard}/{action=Index}/{id?}");
    pattern: "{controller=Authenticate}/{action=Login}/{id?}");
app.Run();
