using CandyWebMVC.Data;
using CandyWebMVC.Helper;
using CandyWebMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Adicione serviços ao contêiner.
//builder.Services.AddControllersWithViews();
//builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddScoped<CartHelper>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.IsEssential = true;  // Indica que o cookie de sessão é essencial para a aplicação.
});

// Configuração do Entity Framework e Identity
builder.Services.AddDbContext<Context>(options =>
    options.UseMySql("server=localhost;initial catalog=Product;uid=root;pwd=1234567",
    Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.38-mysql")));

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
    options.SlidingExpiration = true;
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        builder => builder
            .WithOrigins("http://localhost:4200") // URL do Angular durante o desenvolvimento
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

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

app.UseCors("AllowAngularApp"); // Deve estar após UseRouting e antes de UseAuthentication

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
