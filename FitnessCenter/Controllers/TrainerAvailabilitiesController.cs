using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;

namespace FitnessCenter.Controllers
{
    // Class seviyesinde [Authorize] var: Yani sadece giriş yapmış (üye veya admin) kişiler girebilir.
    [Authorize]
    public class TrainerAvailabilitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerAvailabilitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrainerAvailabilities
        // DÜZELTME: [Authorize(Roles = "Admin")] SİLİNDİ.
        // Artık giriş yapan herkes (Üyeler dahil) burayı görebilir.
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.TrainerAvailabilities.Include(t => t.Trainer);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: TrainerAvailabilities/Details/5
        // DÜZELTME: [Authorize(Roles = "Admin")] SİLİNDİ.
        // Detayları da herkes görebilsin.
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerAvailability = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainerAvailability == null)
            {
                return NotFound();
            }

            return View(trainerAvailability);
        }

        // --- AŞAĞIDAKİLER SADECE ADMIN İÇİN (AYNEN KALIYOR) ---

        // GET: TrainerAvailabilities/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["TrainerId"] = new SelectList(
                _context.Trainers.Where(t => t.IsActive),
                "Id",
                "FirstName"
            );

            return View();
        }

        // POST: TrainerAvailabilities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,TrainerId,DayOfWeek,StartTime,EndTime")] TrainerAvailability trainerAvailability)
        {
            if (ModelState.IsValid)
            {
                _context.Add(trainerAvailability);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "Id", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        // GET: TrainerAvailabilities/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerAvailability = await _context.TrainerAvailabilities.FindAsync(id);
            if (trainerAvailability == null)
            {
                return NotFound();
            }
            // Edit sayfasında Dropdown'da hocanın ismini görmek için "FirstName" seçilmeli (önceki kodda "Id" seçiliydi, düzelttim)
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FirstName", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        // POST: TrainerAvailabilities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TrainerId,DayOfWeek,StartTime,EndTime")] TrainerAvailability trainerAvailability)
        {
            if (id != trainerAvailability.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(trainerAvailability);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerAvailabilityExists(trainerAvailability.Id))
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
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FirstName", trainerAvailability.TrainerId);
            return View(trainerAvailability);
        }

        // GET: TrainerAvailabilities/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainerAvailability = await _context.TrainerAvailabilities
                .Include(t => t.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainerAvailability == null)
            {
                return NotFound();
            }

            return View(trainerAvailability);
        }

        // POST: TrainerAvailabilities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainerAvailability = await _context.TrainerAvailabilities.FindAsync(id);
            if (trainerAvailability != null)
            {
                _context.TrainerAvailabilities.Remove(trainerAvailability);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerAvailabilityExists(int id)
        {
            return _context.TrainerAvailabilities.Any(e => e.Id == id);
        }
    }
}