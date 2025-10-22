using ContractMonthlyClaimSystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddApplicationServices(); // registers IClaimStore, IFileGuard, IUserStore

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(o =>
    {
        o.LoginPath = "/Account/Login";
        o.AccessDeniedPath = "/Account/Denied";
        o.ExpireTimeSpan = TimeSpan.FromHours(2);
    });

var app = builder.Build();

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var userStore = scope.ServiceProvider.GetRequiredService<IUserStore>();
    await userStore.SeedAsync(); // Buhle & Mike
}

app.Run();
