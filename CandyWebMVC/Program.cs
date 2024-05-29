using CandyWebMVC.Data;
using CandyWebMVC.Helper;
using CandyWebMVC.Models;
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
builder.Services.AddSession();

// Configuração do Entity Framework e Identity
builder.Services.AddDbContext<Context>
    (options => options.UseMySql("server=localhost;initial catalog=Product;uid=root;pwd=1234567",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.32-mysql")));

// Adicionando suporte a autenticação com cookies
builder.Services.AddAuthentication("AppCookie").AddCookie("AppCookie", options =>
{
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.LoginPath = "/Auth/Login";  // Direciona para Login se não autenticado
    options.LogoutPath = "/Auth/Logout"; // Direciona para Logout
    options.SlidingExpiration = true; // Renova o tempo de expiração do cookie se o usuário permanecer ativo
});

// Continua a configuração do serviço
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure o pipeline de requisição HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
