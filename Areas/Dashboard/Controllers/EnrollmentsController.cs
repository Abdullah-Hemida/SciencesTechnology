using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciencesTechnology.Data;
using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Dashboard;
using System.Linq;
using System.Threading.Tasks;

namespace SciencesTechnology.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = "Admin")]
    public class EnrollmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.Student)
                .Select(e => new EnrollmentViewModel
                {
                    Id = e.Id,
                    StudentName = e.Student.FirstName + " " + e.Student.LastName,
                    StudentImagePath = string.IsNullOrEmpty(e.Student.ProfileImagePath) ? "/images/default-student.jpg" : e.Student.ProfileImagePath,
                    CourseTitle = e.Course.Title,
                    EnrollmentDate = e.EnrollmentDate,
                    IsConfirmed = e.IsConfirmed,
                    Grade = e.Grade
                })
                .ToListAsync();

            return View(enrollments);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(string id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound();

            enrollment.IsConfirmed = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
                return NotFound();

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}

