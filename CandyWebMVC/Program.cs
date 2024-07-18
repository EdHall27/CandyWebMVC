using CandyWebMVC.Data;
using CandyWebMVC.Helper;
using CandyWebMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicione serviços ao contêiner.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddScoped<CartHelper>();
builder.Services.AddSession(
    options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.IsEssential = true;  // Indica que o cookie de sessão é essencial para a aplicação.
});

// Configuração do Entity Framework e Identity
builder.Services.AddDbContext<Context>
    (options => options.UseMySql("server=localhost;initial catalog=Product;uid=root;pwd=1234567",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.32-mysql")));

// Adicionando suporte a autenticação com cookies
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.LoginPath = "/Auth/Login";
    options.LogoutPath = "/Auth/Logout";
    //options.AccessDeniedPath = "/Auth/LoginRequired";
    options.SlidingExpiration = true;
});

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure o pipeline de requisição HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapRazorPages();
});


app.Run();
