using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessApp.Data;
using FitnessApp.Models;
using Microsoft.AspNetCore.Authorization;

// Namespace'i Areas.Admin olarak düzelttim, doğrusu budur.
namespace WEB_PROGRAMLAMA_PROJESİ.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HizmetController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HizmetController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Hizmet
        public async Task<IActionResult> Index()
        {
            return View(await _context.Hizmetler.ToListAsync());
        }

        // GET: Admin/Hizmet/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.Hizmetler
                .FirstOrDefaultAsync(m => m.HizmetId == id);
            if (hizmet == null)
            {
                return NotFound();
            }

            return View(hizmet);
        }

        // GET: Admin/Hizmet/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Hizmet/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HizmetId,Ad,SureDk,Ucret")] Hizmet hizmet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hizmet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(hizmet);
        }

        // GET: Admin/Hizmet/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet == null)
            {
                return NotFound();
            }
            return View(hizmet);
        }

        // POST: Admin/Hizmet/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HizmetId,Ad,SureDk,Ucret")] Hizmet hizmet)
        {
            if (id != hizmet.HizmetId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hizmet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HizmetExists(hizmet.HizmetId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(hizmet);
        }

        // GET: Admin/Hizmet/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hizmet = await _context.Hizmetler
                .FirstOrDefaultAsync(m => m.HizmetId == id);
            if (hizmet == null)
            {
                return NotFound();
            }

            return View(hizmet);
        }

        // POST: Admin/Hizmet/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hizmet = await _context.Hizmetler.FindAsync(id);
            if (hizmet != null)
            {
                _context.Hizmetler.Remove(hizmet);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HizmetExists(int id)
        {
            return _context.Hizmetler.Any(e => e.HizmetId == id);
        }
    }
}