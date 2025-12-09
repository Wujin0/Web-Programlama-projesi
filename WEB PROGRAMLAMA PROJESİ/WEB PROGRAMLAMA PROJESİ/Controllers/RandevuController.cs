using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace FitnessApp.Controllers
{
    [Authorize]
    public class RandevuController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public RandevuController(ApplicationDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Al()
        {
            // Dropdownları doldur
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "AntrenorId", "AdSoyad");
            ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "HizmetId", "Ad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Al([Bind("RandevuId,TarihSaat,AntrenorId,HizmetId")] Randevu randevu)
        {
            // Validasyon temizliği
            ModelState.Remove("User");
            ModelState.Remove("UserId");
            ModelState.Remove("Antrenor");
            ModelState.Remove("Hizmet");

            // --- YENİ EKLENEN KISIM: BOŞ KONTROLÜ ---
            // Eğer kullanıcı seçim yapmazsa değer 0 gelir.
            if (randevu.AntrenorId == 0)
            {
                TempData["Hata"] = "Lütfen listeden bir <b>Eğitmen</b> seçiniz.";
                return ListeleriDoldurVeDondur(randevu);
            }

            if (randevu.HizmetId == 0)
            {
                TempData["Hata"] = "Lütfen almak istediğiniz <b>Hizmeti</b> seçiniz.";
                return ListeleriDoldurVeDondur(randevu);
            }
            // ----------------------------------------

            if (ModelState.IsValid)
            {
                // KURAL 1: GEÇMİŞ TARİH KONTROLÜ
                if (randevu.TarihSaat < DateTime.Now)
                {
                    TempData["Hata"] = "Geçmiş bir tarihe randevu alamazsınız.";
                    return ListeleriDoldurVeDondur(randevu);
                }

                var secilenAntrenor = await _context.Antrenorler.FindAsync(randevu.AntrenorId);
                var secilenHizmet = await _context.Hizmetler.FindAsync(randevu.HizmetId);

                if (secilenAntrenor == null || secilenHizmet == null)
                {
                    TempData["Hata"] = "Seçim hatalı. Lütfen sayfayı yenileyip tekrar deneyin.";
                    return ListeleriDoldurVeDondur(randevu);
                }

                // UZMANLIK KONTROLÜ
                string hocaUzmanlik = secilenAntrenor.UzmanlikAlani.ToLower();
                string hizmetAdi = secilenHizmet.Ad.ToLower();

                if (!hocaUzmanlik.Contains(hizmetAdi))
                {
                    TempData["Hata"] = $"Seçtiğiniz antrenör ({secilenAntrenor.AdSoyad}) <b>'{secilenHizmet.Ad}'</b> dersi vermiyor.<br>Uzmanlık alanı: {secilenAntrenor.UzmanlikAlani}";
                    return ListeleriDoldurVeDondur(randevu);
                }

                // SAAT KONTROLÜ
                TimeSpan randevuBaslangic = randevu.TarihSaat.TimeOfDay;
                TimeSpan hizmetSuresi = TimeSpan.FromMinutes(secilenHizmet.SureDk);
                TimeSpan randevuBitis = randevuBaslangic.Add(hizmetSuresi);

                TimeSpan hocaBaslangic = TimeSpan.Parse(secilenAntrenor.CalismaSaatiBaslangic);
                TimeSpan hocaBitis = TimeSpan.Parse(secilenAntrenor.CalismaSaatiBitis);

                if (randevuBaslangic < hocaBaslangic || randevuBitis > hocaBitis)
                {
                    string randevuAraligi = $"{randevuBaslangic:hh\\:mm} - {randevuBitis:hh\\:mm}";
                    string hocaAraligi = $"{hocaBaslangic:hh\\:mm} - {hocaBitis:hh\\:mm}";

                    TempData["Hata"] = $"<div class='text-start'><b>Saat Uyuşmazlığı!</b><br>" +
                                       $"Sizin Randevunuz: <b>{randevuAraligi}</b><br>" +
                                       $"Hocanın Mesaisi: <b>{hocaAraligi}</b><br>" +
                                       $"Lütfen hocanın çalışma saatleri içinde bir zaman seçin.</div>";
                    return ListeleriDoldurVeDondur(randevu);
                }

                // ÇAKIŞMA KONTROLÜ
                DateTime baslangicZamani = randevu.TarihSaat;
                DateTime bitisZamani = baslangicZamani.AddMinutes(secilenHizmet.SureDk);

                var cakismaVarMi = await _context.Randevular
                    .Include(r => r.Hizmet)
                    .Where(r => r.AntrenorId == randevu.AntrenorId)
                    .Where(r => r.RandevuId != randevu.RandevuId)
                    .AnyAsync(r =>
                        (r.TarihSaat < bitisZamani) &&
                        (r.TarihSaat.AddMinutes(r.Hizmet.SureDk) > baslangicZamani)
                    );

                if (cakismaVarMi)
                {
                    TempData["Hata"] = "Seçtiğiniz saatte antrenörün başka bir randevusu var. Lütfen başka bir saat seçin.";
                    return ListeleriDoldurVeDondur(randevu);
                }

                // KAYDET
                var user = await _userManager.GetUserAsync(User);
                randevu.UserId = user.Id;
                randevu.OnaylandiMi = false;

                _context.Add(randevu);
                await _context.SaveChangesAsync();

                TempData["Basarili"] = "Randevunuz başarıyla oluşturuldu! Onay bekliyor.";
                return RedirectToAction(nameof(Randevularim));
            }

            // Eğer yukarıdaki kontrollere takılmadan buraya düşerse genel hata ver
            TempData["Hata"] = "Lütfen formdaki tüm alanları doldurunuz.";
            return ListeleriDoldurVeDondur(randevu);
        }

        private IActionResult ListeleriDoldurVeDondur(Randevu randevu)
        {
            ViewData["AntrenorId"] = new SelectList(_context.Antrenorler, "AntrenorId", "AdSoyad", randevu.AntrenorId);
            ViewData["HizmetId"] = new SelectList(_context.Hizmetler, "HizmetId", "Ad", randevu.HizmetId);
            return View(randevu);
        }

        public async Task<IActionResult> Randevularim()
        {
            var user = await _userManager.GetUserAsync(User);
            var randevular = await _context.Randevular
                .Include(r => r.Antrenor)
                .Include(r => r.Hizmet)
                .Where(r => r.UserId == user.Id)
                .OrderByDescending(r => r.TarihSaat)
                .ToListAsync();

            return View(randevular);
        }
    }
}