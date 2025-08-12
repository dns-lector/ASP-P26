using ASP_P26.Data;
using ASP_P26.Middleware.Auth;
using ASP_P26.Services.Email;
using ASP_P26.Services.Jwt;
using ASP_P26.Services.Kdf;
using ASP_P26.Services.Random;
using ASP_P26.Services.Time;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

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

builder.Services.AddDbContext<DataContext>(
    options => options.UseSqlServer(
        builder.Configuration.GetConnectionString("LocalDb"))
);


builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


var app = builder.Build();

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
/* Д.З. Реалізувати систему автентифікації та авторизації
 * у власному курсовому проєкті.
 */
