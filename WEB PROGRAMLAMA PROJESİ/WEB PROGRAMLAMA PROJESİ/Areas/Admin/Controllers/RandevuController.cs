using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;

namespace FitnessApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RandevuController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TÜM RANDEVULARI LİSTELE
        public async Task<IActionResult> Index()
        {
            var randevular = await _context.Randevular
                .Include(r => r.Antrenor) // Hangi hoca?
                .Include(r => r.Hizmet)   // Hangi ders?
                .Include(r => r.User)     // Kim almış? (AppUser tablosu)
                .OrderByDescending(r => r.TarihSaat) // En yeni en üstte
                .ToListAsync();

            return View(randevular);
        }

        // 2. RANDEVUYU ONAYLA (Butona basınca çalışır)
        [HttpPost]
        public async Task<IActionResult> Onayla(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                randevu.OnaylandiMi = true; // Onaylandı işaretle
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index)); // Sayfayı yenile
        }

        // 3. RANDEVUYU SİL / İPTAL ET
        [HttpPost]
        public async Task<IActionResult> Sil(int id)
        {
            var randevu = await _context.Randevular.FindAsync(id);
            if (randevu != null)
            {
                _context.Randevular.Remove(randevu); // Veritabanından sil
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}