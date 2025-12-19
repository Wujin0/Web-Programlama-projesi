using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;

namespace FitnessApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AntrenorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AntrenorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME
        public async Task<IActionResult> Index()
        {
            return View(await _context.Antrenorler.ToListAsync());
        }

        // 2. EKLEME SAYFASI
        public IActionResult Create()
        {
            return View();
        }

        // 3. EKLEME İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind kısmına saatleri ekledik
        public async Task<IActionResult> Create([Bind("AntrenorId,AdSoyad,UzmanlikAlani,CalismaSaatiBaslangic,CalismaSaatiBitis")] Antrenor antrenor, IFormFile? resimDosyasi)
        {
            // Validasyon hatalarını temizle
            ModelState.Remove("FotoUrl");
            ModelState.Remove("Randevular");

            if (ModelState.IsValid)
            {
                try
                {
                    if (resimDosyasi != null)
                    {
                        string uzanti = Path.GetExtension(resimDosyasi.FileName);
                        string yeniIsim = Guid.NewGuid().ToString() + uzanti;
                        string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/antrenorler");

                        if (!Directory.Exists(klasorYolu)) Directory.CreateDirectory(klasorYolu);

                        using (var stream = new FileStream(Path.Combine(klasorYolu, yeniIsim), FileMode.Create))
                        {
                            await resimDosyasi.CopyToAsync(stream);
                        }
                        antrenor.FotoUrl = "/img/antrenorler/" + yeniIsim;
                    }
                    else
                    {
                        antrenor.FotoUrl = "https://via.placeholder.com/150";
                    }

                    _context.Add(antrenor);
                    await _context.SaveChangesAsync();
                    TempData["Basarili"] = "Antrenör başarıyla eklendi!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    TempData["Hata"] = "Antrenör eklenirken bir hata oluştu. Lütfen tekrar deneyin.";
                }
            }
            return View(antrenor);
        }

        // 4. DÜZENLEME SAYFASI
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var antrenor = await _context.Antrenorler.FindAsync(id);
            if (antrenor == null) return NotFound();
            return View(antrenor);
        }

        // 5. DÜZENLEME İŞLEMİ (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind kısmına saatleri ekledik
        public async Task<IActionResult> Edit(int id, [Bind("AntrenorId,AdSoyad,UzmanlikAlani,FotoUrl,CalismaSaatiBaslangic,CalismaSaatiBitis")] Antrenor antrenor, IFormFile? resimDosyasi)
        {
            if (id != antrenor.AntrenorId) return NotFound();

            ModelState.Remove("FotoUrl");
            ModelState.Remove("Randevular");

            if (ModelState.IsValid)
            {
                try
                {
                    if (resimDosyasi != null)
                    {
                        string uzanti = Path.GetExtension(resimDosyasi.FileName);
                        string yeniIsim = Guid.NewGuid().ToString() + uzanti;
                        string klasorYolu = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/antrenorler");

                        using (var stream = new FileStream(Path.Combine(klasorYolu, yeniIsim), FileMode.Create))
                        {
                            await resimDosyasi.CopyToAsync(stream);
                        }
                        antrenor.FotoUrl = "/img/antrenorler/" + yeniIsim;
                    }

                    _context.Update(antrenor);
                    await _context.SaveChangesAsync();
                    TempData["Basarili"] = "Antrenör başarıyla güncellendi!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Antrenorler.Any(e => e.AntrenorId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(antrenor);
        }

        // 6. SİLME SAYFASI
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var antrenor = await _context.Antrenorler.FirstOrDefaultAsync(m => m.AntrenorId == id);
            if (antrenor == null) return NotFound();
            return View(antrenor);
        }

        // 7. SİLME İŞLEMİ (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var antrenor = await _context.Antrenorler.FindAsync(id);
            if (antrenor != null)
            {
                _context.Antrenorler.Remove(antrenor);
                await _context.SaveChangesAsync();
                TempData["Basarili"] = "Antrenör başarıyla silindi!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}