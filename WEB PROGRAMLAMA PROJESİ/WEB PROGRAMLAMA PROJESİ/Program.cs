using FitnessApp.Data;
using FitnessApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsý (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity (Üyelik) Servisi
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// --- ADIM 1: ÞÝFRE KURALLARINI ESNETME (BURASI YENÝ) ---
// Admin þifresi "sau" olabilsin diye kurallarý gevþetiyoruz.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;          // Rakam zorunluluðu yok
    options.Password.RequireLowercase = false;      // Küçük harf zorunluluðu yok
    options.Password.RequireUppercase = false;      // Büyük harf zorunluluðu yok
    options.Password.RequireNonAlphanumeric = false;// Sembol (!,*,.) zorunluluðu yok
    options.Password.RequiredLength = 3;            // En az 3 karakter yeterli
});
// --------------------------------------------------------

builder.Services.AddControllersWithViews();

var app = builder.Build();

// HTTP request pipeline ayarlarý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kimlik doðrulama (Giriþ yapma)
app.UseAuthorization();  // Yetkilendirme (Rol kontrolü)

// --- SEED DATA (OTOMATÝK ADMÝN) ---
// Bu kýsmý bir sonraki adýmda ekleyeceðiz ama þimdilik yerini hazýr tutuyoruz.
// Eðer DbSeeder kodunu yazdýysan buraya eklemeyi unutma.
// ----------------------------------



// Admin Paneli Rotasý
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
);

// (Burada eski default rota durmaya devam edecek)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- SEED DATA BAÞLANGIÇ ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // Hata olursa program patlamasýn diye try-catch içine alýyoruz
    try
    {
        await FitnessApp.Data.DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        // Ýleride loglama yapýlabilir, þimdilik boþ geçelim.
        Console.WriteLine(ex.Message);
    }
}
// --- SEED DATA BÝTÝÞ ---

app.Run(); // Bu satýr zaten vardý, kodlarý bunun ÜSTÜNE ekle.

app.Run();