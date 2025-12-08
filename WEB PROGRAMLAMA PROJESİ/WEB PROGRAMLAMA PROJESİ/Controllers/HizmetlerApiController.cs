using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;

namespace FitnessApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HizmetlerApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HizmetlerApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. TÜM HİZMETLERİ GETİR (GET: api/HizmetlerApi)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hizmet>>> GetHizmetler()
        {
            // LINQ Kullanımı: Veritabanından listeyi çekiyor
            return await _context.Hizmetler.ToListAsync();
        }

        // 2. FİYATA GÖRE FİLTRELE (GET: api/HizmetlerApi/Filtrele?maxFiyat=500)
        // Ödevde istenen "LINQ ile filtreleme" şartını burası sağlıyor.
        [HttpGet("Filtrele")]
        public async Task<ActionResult<IEnumerable<Hizmet>>> GetUcuzHizmetler(decimal maxFiyat)
        {
            // LINQ Sorgusu: Fiyatı girilen değerden düşük olanları filtrele
            var filtreliListe = await _context.Hizmetler
                                      .Where(h => h.Ucret <= maxFiyat)
                                      .ToListAsync();

            if (!filtreliListe.Any())
            {
                return NotFound("Bu fiyata uygun hizmet bulunamadı.");
            }

            return Ok(filtreliListe);
        }
    }
}