using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciencesTechnology.Data;
using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Website;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SciencesTechnology.Areas.Website.Controllers
{
    [Area("Website")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CoursesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
        // GET: Courses
        public async Task<IActionResult> Index(string? departmentId)
        {
            // Get departments for filter buttons
            ViewBag.Departments = await _context.Departments
                .Select(d => new { d.Id, d.DepartmentName })
                .ToListAsync();

            // Get Teacher Role ID
            var teacherRoleId = await _context.Roles
                .Where(r => r.Name == "Teacher")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();           

            if (teacherRoleId == null)
            {
                return Problem("Teacher role not found.");
            }

            // Fetch courses where teacher is valid
            var coursesQuery = _context.Courses.Where(c => c.IsActive)
                .Include(c => c.Department)
                .Include(c => c.Teacher) // Eager load Teacher navigation
                .Where(c => _context.UserRoles
                    .Any(ur => ur.UserId == c.TeacherId && ur.RoleId == teacherRoleId)); // Filter valid teachers
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Apply department filter if provided
            if (!string.IsNullOrEmpty(departmentId))
            {
                coursesQuery = coursesQuery.Where(c => c.DepartmentId == departmentId);
            }

            // Project to view model
            var courses = await coursesQuery
                .Select(c => new CourseViewModel
                {
                    Id = c.Id,
                    Title = c.Title!,
                    Description = c.Description,
                    DepartmentName = c.Department.DepartmentName,
                    CourseCode = c.CourseCode,
                    Credits = c.Credits,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    IsActive = c.IsActive,
                    CourseImagePath = c.CourseImagePath,
                    TeacherFirstName = c.Teacher != null ? c.Teacher.FirstName : "Unknown", // Handle null Teacher
                    TeacherImagePath =  c.Teacher.ProfileImagePath,
                    // Check if the user is enrolled and if it is confirmed
                    IsEnrolled = _context.Enrollments.Any(e => e.CourseId == c.Id && e.StudentId == userId),
                    IsConfirmed = _context.Enrollments.Any(e => e.CourseId == c.Id && e.StudentId == userId && e.IsConfirmed)
                })
                .ToListAsync();

            return View(courses);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var course = await _context.Courses
                .Include(c => c.Department)
                .Where(c => c.Id == id)
                .Select(c => new CourseDetailsViewModel
                {
                    Id = c.Id,
                    Title = c.Title!,
                    Description = c.Description,
                    DepartmentName = c.Department.DepartmentName,
                    CourseCode = c.CourseCode,
                    Credits = c.Credits,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    IsActive = c.IsActive,
                    CourseImagePath = c.CourseImagePath,
                    TeacherFirstName = c.Teacher != null ? c.Teacher.FirstName : "Unknown", // Handle null Teacher
                    TeacherImagePath = c.Teacher.ProfileImagePath,
                    // Check if the user is enrolled and if it is confirmed
                    IsEnrolled = _context.Enrollments.Any(e => e.CourseId == c.Id && e.StudentId == userId),
                    IsConfirmed = _context.Enrollments.Any(e => e.CourseId == c.Id && e.StudentId == userId && e.IsConfirmed)
                })
                .FirstOrDefaultAsync();

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var course = await _context.Courses.FindAsync(id);
            if (course == null || !course.IsActive || DateTime.UtcNow >= course.StartDate)
                return BadRequest("Enrollment is closed for this course.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var alreadyEnrolled = await _context.Enrollments.AnyAsync(e => e.CourseId == id && e.StudentId == userId);
            if (alreadyEnrolled)
                return BadRequest("You are already enrolled in this course.");

            var enrollment = new Enrollment
            {
                CourseId = id,
                StudentId = userId,
                EnrollmentDate = DateTime.UtcNow
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

    }
}

