namespace FitnessApp.Models
{
    public class Randevu
    {
        public int RandevuId { get; set; }

        public string? UserId { get; set; } // Randevuyu alan üye
        public AppUser? User { get; set; }

        public int AntrenorId { get; set; }
        public Antrenor? Antrenor { get; set; }

        public int HizmetId { get; set; }
        public Hizmet? Hizmet { get; set; }

        public DateTime TarihSaat { get; set; }
        public bool OnaylandiMi { get; set; } // Admin onayı için
    }
}