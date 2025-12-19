using Microsoft.AspNetCore.Identity;

namespace FitnessApp.Models
{
    public class AppUser : IdentityUser
    {
        public string? Ad { get; set; }
        public string? Soyad { get; set; }
        public int? Boy { get; set; }  // AI için gerekli
        public int? Kilo { get; set; } // AI için gerekli
        public DateTime DogumTarihi { get; set; }
        public string? Cinsiyet { get; set; }
    }
}