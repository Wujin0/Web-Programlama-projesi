using FitnessApp.Data;
using FitnessApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritaban� Ba�lant�s� (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Identity (�yelik) Servisi
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// --- ADIM 1: ��FRE KURALLARINI ESNETME (BURASI YEN�) ---
// Admin �ifresi "sau" olabilsin diye kurallar� gev�etiyoruz.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;          // Rakam zorunlulu�u yok
    options.Password.RequireLowercase = false;      // K���k harf zorunlulu�u yok
    options.Password.RequireUppercase = false;      // B�y�k harf zorunlulu�u yok
    options.Password.RequireNonAlphanumeric = false;// Sembol (!,*,.) zorunlulu�u yok
    options.Password.RequiredLength = 3;            // En az 3 karakter yeterli
});
// --------------------------------------------------------

builder.Services.AddControllersWithViews();

var app = builder.Build();

// HTTP request pipeline ayarlar�
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Kimlik do�rulama (Giri� yapma)
app.UseAuthorization();  // Yetkilendirme (Rol kontrol�)

// --- SEED DATA (OTOMAT�K ADM�N) ---
// Bu k�sm� bir sonraki ad�mda ekleyece�iz ama �imdilik yerini haz�r tutuyoruz.
// E�er DbSeeder kodunu yazd�ysan buraya eklemeyi unutma.
// ----------------------------------



// Admin Paneli Rotas�
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

// --- SEED DATA BAŞLANGI ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    // Hata olursa program patlmasın diye try-catch içine alıyoruz
    try
    {
        await FitnessApp.Data.DbSeeder.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        // İleride loglama yapılabilir, şimdilik boş geçelim.
        Console.WriteLine(ex.Message);
    }
}
// --- SEED DATA BİTİŞ ---

app.Run();