using ASP_P26.Data;
using ASP_P26.Middleware.Auth;
using ASP_P26.Middleware.Cart;
using ASP_P26.Services.Email;
using ASP_P26.Services.Jwt;
using ASP_P26.Services.Kdf;
using ASP_P26.Services.Random;
using ASP_P26.Services.Storage;
using ASP_P26.Services.Time;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("emailsettings.json");

// Add services to the container.
builder.Services.AddControllersWithViews();

//builder.Services.AddSingleton<ITimeService, SecTimeService>();
builder.Services.AddSingleton<IRandomService, DefaultRandomService>();
builder.Services.AddSingleton<ITimeService, MillisecTimeService>();
builder.Services.AddSingleton<IKdfService, PbKdfService>();
builder.Services.AddSingleton<IEmailService, GmailService>();
builder.Services.AddSingleton<IJwtService, JwtServiceV1>();
builder.Services.AddSingleton<IStorageService, DiskStorageService>();

builder.Services.AddDbContext<DataContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("LocalDb"))
);
builder.Services.AddScoped<DataAccessor>();


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(1000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

app.UseRequestLocalization(opt =>
{
    opt.DefaultRequestCulture = new("en-US");
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();

app.UseAuthSession();
// app.UseAuthToken();
app.UseAuthJwt();

app.UseUserCart();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    await db.Database.MigrateAsync();
}

app.Run();
/* Д.З. Завершити роботу з завантаженням/вивантаженням файлів,
 * зокрема, зображень; сторінки типу АРМ (dashboard) з обмеженням доступу по ролях
 * Додати сутності основного контенту, підготувати АРІ контролери для них
 * у власному курсовому проєкті.
 */
