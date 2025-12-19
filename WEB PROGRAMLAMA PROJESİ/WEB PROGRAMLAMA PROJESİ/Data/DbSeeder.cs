using FitnessApp.Models;
using Microsoft.AspNetCore.Identity;

namespace FitnessApp.Data
{
    public static class DbSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider service)
        {
            // Kullanıcı ve Rol yöneticilerini çağırıyoruz
            var userManager = service.GetService<UserManager<AppUser>>();
            var roleManager = service.GetService<RoleManager<IdentityRole>>();

            if (userManager == null || roleManager == null)
            {
                throw new InvalidOperationException("UserManager or RoleManager is not available.");
            }

            // 1. ROLLERİ OLUŞTUR (Admin ve Uye)
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Uye"));

            // 2. ADMİN KULLANCISINI OLUŞTUR
            // Öğrenci numaranı aşağıya yazmayı unutma!
            string adminEmail = "ogrencinumarasi@sakarya.edu.tr";

            var user = await userManager.FindByEmailAsync(adminEmail);
            if (user == null)
            {
                user = new AppUser()
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Ad = "Sistem",
                    Soyad = "Admin",
                    EmailConfirmed = true,
                    DogumTarihi = DateTime.Now
                };

                // Şifreyi "sau" olarak belirliyoruz
                await userManager.CreateAsync(user, "sau");

                // Kullanıcıya Admin rolünü atıyoruz
                await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}