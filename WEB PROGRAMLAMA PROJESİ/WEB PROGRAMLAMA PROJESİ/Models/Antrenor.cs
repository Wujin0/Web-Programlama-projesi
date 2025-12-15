using System.ComponentModel.DataAnnotations;
// antrenor saatleri eklendi
namespace FitnessApp.Models
{
    public class Antrenor
    {
        [Key]
        public int AntrenorId { get; set; }

        [Required(ErrorMessage = "Ad Soyad zorunludur.")]
        [Display(Name = "Ad Soyad")]
        public string AdSoyad { get; set; }

        [Required(ErrorMessage = "Uzmanlık alanı zorunludur.")]
        [Display(Name = "Uzmanlık Alanı")]
        public string UzmanlikAlani { get; set; }

        [Display(Name = "Fotoğraf")]
        public string? FotoUrl { get; set; }

        // --- YENİ EKLENEN ÖZELLİKLER: ÇALIŞMA SAATLERİ ---
        [Display(Name = "Mesai Başlangıç")]
        public string CalismaSaatiBaslangic { get; set; } = "09:00"; // Varsayılan

        [Display(Name = "Mesai Bitiş")]
        public string CalismaSaatiBitis { get; set; } = "17:00"; // Varsayılan

        // İlişkiler
        public List<Randevu>? Randevular { get; set; }
    }
}