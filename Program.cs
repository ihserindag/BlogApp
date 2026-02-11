using BlogApp.Data.Abstract;
using BlogApp.Data.Concreate;
using BlogApp.Data.Concreate.EfCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BlogContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("mysql_connection");
    Console.WriteLine($"MySQL Connection: {(string.IsNullOrEmpty(connectionString) ? "BOŞ - HATA!" : "Bağlantı dizesi bulundu")}");
    var serverVersion = new MySqlServerVersion(new Version(8, 0, 0));
    options.UseMySql(connectionString, serverVersion);
});

builder.Services.AddScoped<IPostRepository, EfPostRepository>();
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(
    options =>
    {
        options.LoginPath="/Users/Login";
    }
); //login işlemleri

var app = builder.Build();

app.UseRouting();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

try
{
    SeedData.TestVerileriniDoldur(app);
}
catch (Exception ex)
{
    Console.WriteLine($"SeedData Hatası: {ex.Message}");
    Console.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
}

app.MapControllerRoute(
    name: "post-details",
    pattern: "posts/detail/{url}",
    defaults: new { controller = "Posts", action = "Details" }
);

app.MapControllerRoute(
    name: "posts_by_tag",
    pattern: "posts/tag/{tag}",
    defaults: new { controller = "Posts", action = "Index" }
);

app.MapControllerRoute(
    name: "user_profile",
    pattern: "profile/{username}",
    defaults: new { controller = "Users", action = "Profile" }
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}");

app.Run();
