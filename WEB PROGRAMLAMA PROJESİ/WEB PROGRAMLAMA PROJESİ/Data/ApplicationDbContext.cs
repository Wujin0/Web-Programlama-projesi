using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Models;

namespace FitnessApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Antrenor> Antrenorler { get; set; }
        public DbSet<Hizmet> Hizmetler { get; set; }
        public DbSet<Randevu> Randevular { get; set; }

        // BU KISMI EKLİYORUZ:
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity ayarları için bu satır ŞARTTIR.

            // Hizmet tablosundaki Ucret alanı için ayar:
            // decimal(18,2) -> Toplam 18 basamak, virgülden sonra 2 basamak (Örn: 150.99)
            builder.Entity<Hizmet>()
                .Property(h => h.Ucret)
                .HasColumnType("decimal(18,2)");
        }
    }
}