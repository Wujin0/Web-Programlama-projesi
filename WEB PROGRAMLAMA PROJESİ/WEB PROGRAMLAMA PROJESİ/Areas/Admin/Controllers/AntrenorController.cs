using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace WEB_PROGRAMLAMA_PROJESİ.Areas.Admin.Controllers
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

        // GET: Admin/Antrenor
        public async Task<IActionResult> Index()
        {
            return View(await _context.Antrenorler.ToListAsync());
        }

        // GET: Admin/Antrenor/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Antrenor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AntrenorId,AdSoyad,UzmanlikAlani,FotoUrl")] Antrenor antrenor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(antrenor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(antrenor);
        }

        // GET: Admin/Antrenor/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var antrenor = await _context.Antrenorler.FindAsync(id);
            if (antrenor == null) return NotFound();

            return View(antrenor);
        }

        // POST: Admin/Antrenor/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AntrenorId,AdSoyad,UzmanlikAlani,FotoUrl")] Antrenor antrenor)
        {
            if (id != antrenor.AntrenorId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(antrenor);
                    await _context.SaveChangesAsync();
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

        // GET: Admin/Antrenor/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var antrenor = await _context.Antrenorler.FirstOrDefaultAsync(m => m.AntrenorId == id);
            if (antrenor == null) return NotFound();

            return View(antrenor);
        }

        // POST: Admin/Antrenor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var antrenor = await _context.Antrenorler.FindAsync(id);
            if (antrenor != null) _context.Antrenorler.Remove(antrenor);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}