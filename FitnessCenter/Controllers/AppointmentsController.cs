using System;
using System.Linq;
using System.Threading.Tasks;
using FitnessCenter.Data;
using FitnessCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenter.Controllers
{
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AppointmentsController(ApplicationDbContext context,
                                      UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ---------------------------------------------------------
        //  LISTELER
        // ---------------------------------------------------------

        // Admin + Member tüm randevuları görür
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .Include(a => a.Service);

            return View(await applicationDbContext.ToListAsync());
        }

        // Üyenin sadece kendi randevularını gördüğü sayfa
        [Authorize]
        public async Task<IActionResult> MyAppointments()
        {
            var email = User.Identity!.Name;

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == email);

            if (member == null)
                return NotFound("Bu kullanıcıya bağlı üye kaydı bulunamadı.");

            var list = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .Where(a => a.MemberId == member.Id)
                .ToListAsync();

            return View(list);
        }

        // ---------------------------------------------------------
        //  DETAILS
        // ---------------------------------------------------------

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        // ---------------------------------------------------------
        //  CREATE (Admin + Member)
        // ---------------------------------------------------------

        // GET: Appointments/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            var isAdmin = User.IsInRole("Admin");

            await FillDropdownsForCreateAsync(new Appointment(), isAdmin);

            return View();
        }

        // POST: Appointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(
            [Bind("MemberId,TrainerId,ServiceId,StartTime,EndTime")] Appointment appointment)
        {
            var isAdmin = User.IsInRole("Admin");

            // Member ise MemberId'yi zorla kendisine set et
            if (!isAdmin)
            {
                var email = User.Identity!.Name;
                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.Email == email);

                if (member == null)
                {
                    ModelState.AddModelError(string.Empty,
                        "Bu kullanıcıya bağlı üye kaydı bulunamadı.");
                    await FillDropdownsForCreateAsync(appointment, isAdmin);
                    return View(appointment);
                }

                appointment.MemberId = member.Id;
            }

            if (!ModelState.IsValid)
            {
                await FillDropdownsForCreateAsync(appointment, isAdmin);
                return View(appointment);
            }

            // Bitiş saati kontrolü
            if (appointment.EndTime <= appointment.StartTime)
            {
                ModelState.AddModelError(string.Empty,
                    "Bitiş saati başlangıç saatinden sonra olmalıdır.");
                await FillDropdownsForCreateAsync(appointment, isAdmin);
                return View(appointment);
            }

            // 1) Antrenörün o gün için çalışma saati var mı?
            int appointmentDay = (int)appointment.StartTime.DayOfWeek; // 0=Pazar,1=Pts...
            var startTimeOfDay = appointment.StartTime.TimeOfDay;
            var endTimeOfDay = appointment.EndTime.TimeOfDay;

            var availabilities = await _context.TrainerAvailabilities
                .Where(ta => ta.TrainerId == appointment.TrainerId &&
                             ta.DayOfWeek == appointmentDay)
                .ToListAsync();

            bool withinAvailability = availabilities.Any(a =>
                startTimeOfDay >= a.StartTime && endTimeOfDay <= a.EndTime);

            if (!withinAvailability)
            {
                ModelState.AddModelError(string.Empty,
                    "Seçilen saat aralığında antrenör için tanımlı bir çalışma saati yok.");
                await FillDropdownsForCreateAsync(appointment, isAdmin);
                return View(appointment);
            }

            // 2) Aynı anda başka randevu var mı?
            var conflictingAppointments = await _context.Appointments
                .Where(a => a.TrainerId == appointment.TrainerId)
                .Where(a => a.StartTime.Date == appointment.StartTime.Date)
                // Sadece Beklemede + Onaylandı olanlar çakışma sayılır
                .Where(a => a.Status == AppointmentStatus.Pending
                         || a.Status == AppointmentStatus.Approved)
                .Where(a =>
                    appointment.StartTime < a.EndTime &&
                    appointment.EndTime > a.StartTime)
                .ToListAsync();

            if (conflictingAppointments.Any())
            {
                ModelState.AddModelError(string.Empty,
                    "Bu saat aralığında antrenörün başka bir randevusu bulunmaktadır.");
                await FillDropdownsForCreateAsync(appointment, isAdmin);
                return View(appointment);
            }

            // Yeni randevu her zaman "Beklemede" başlar
            appointment.Status = AppointmentStatus.Pending;
            appointment.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm");

            _context.Add(appointment);
            await _context.SaveChangesAsync();

            // Member randevu açtıysa MyAppointments'a, admin açtıysa Index'e dönebiliriz
            if (isAdmin)
                return RedirectToAction(nameof(Index));
            else
                return RedirectToAction(nameof(MyAppointments));
        }

        // ---------------------------------------------------------
        //  EDIT / DELETE  (sadece Admin)
        // ---------------------------------------------------------

        // GET: Appointments/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
                return NotFound();

            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", appointment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FirstName", appointment.TrainerId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);

            return View(appointment);
        }

        // POST: Appointments/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Appointment appointment)
        {
            if (id != appointment.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppointmentExists(appointment.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", appointment.MemberId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers, "Id", "FirstName", appointment.TrainerId);
            ViewData["ServiceId"] = new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);

            return View(appointment);
        }

        // ---------------------------------------------------------
        //  ONAY / RED (sadece Admin)
        // ---------------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            appointment.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            appointment.Status = AppointmentStatus.Rejected;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------
        //  DELETE (sadece Admin)
        // ---------------------------------------------------------

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var appointment = await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .Include(a => a.Service)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (appointment == null)
                return NotFound();

            return View(appointment);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------------------------------------------------------
        //  HELPER METOTLAR
        // ---------------------------------------------------------

        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }

        /// <summary>
        /// Create ekranı için dropdown + member bilgilerini doldurur.
        /// Admin ise tüm üyeleri gösterir, Member ise sadece kendisini hidden olarak yollar.
        /// </summary>
        private async Task FillDropdownsForCreateAsync(Appointment appointment, bool isAdmin)
        {
            if (isAdmin)
            {
                ViewData["MemberId"] =
                    new SelectList(_context.Members, "Id", "FullName", appointment.MemberId);
            }
            else
            {
                var email = User.Identity!.Name;
                var member = await _context.Members.FirstOrDefaultAsync(m => m.Email == email);

                ViewData["CurrentMemberId"] = member?.Id;
                ViewData["CurrentMemberName"] = member?.FullName;
            }

            ViewData["TrainerId"] =
                new SelectList(_context.Trainers, "Id", "FirstName", appointment.TrainerId);
            ViewData["ServiceId"] =
                new SelectList(_context.Services, "Id", "Name", appointment.ServiceId);
        }
    }
}
