namespace FitnessApp.Models
{
    public class Hizmet
    {
        public int HizmetId { get; set; }
        public string? Ad { get; set; } // Pilates, Reformer vb.
        public int SureDk { get; set; } // 45 dk, 60 dk
        public decimal Ucret { get; set; }
    }
}