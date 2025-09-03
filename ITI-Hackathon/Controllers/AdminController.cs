using ITI_Hackathon.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI_Hackathon.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        public AdminController(ApplicationDbContext db) => _db = db;

        // GET: /Admin/PendingDoctors
        public async Task<IActionResult> PendingDoctors()
        {
            var pending = await _db.Doctors
                .Include(d => d.User)
                .Where(d => !d.IsApproved)
                .ToListAsync();

            return View(pending);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDoctor(int id)
        {
            var doctor = await _db.Doctors.FindAsync(id);
            if (doctor == null) return NotFound();

            doctor.IsApproved = true;
            _db.Doctors.Update(doctor);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(PendingDoctors));
        }
    }
}
