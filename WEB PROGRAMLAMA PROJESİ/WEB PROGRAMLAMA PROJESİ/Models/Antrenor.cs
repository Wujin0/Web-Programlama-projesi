namespace FitnessApp.Models
{
    public class Antrenor
    {
        public int AntrenorId { get; set; }
        public string AdSoyad { get; set; }
        public string UzmanlikAlani { get; set; } // Örn: Fitness, Yoga
        public string? FotoUrl { get; set; }

        // İlişkiler
        public ICollection<Randevu> Randevular { get; set; }
    }
}