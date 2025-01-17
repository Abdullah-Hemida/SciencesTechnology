using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SciencesTechnology.Data;
using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Dashboard;

namespace SciencesTechnology.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = "Admin,Teacher")]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
           
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            IQueryable<Course> query = User.IsInRole("Admin")
                ? _context.Courses.Include(c => c.Department).OrderBy(c => c.AddedDate)
                : _context.Courses.Where(c => c.TeacherId == userId)
                                  .Include(c => c.Department)
                                  .OrderBy(c => c.AddedDate);

            var courses = await query.ToListAsync();
            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            // Fetch the teacher (creator) information using TeacherId
            var teacher = await _context.Users.FirstOrDefaultAsync(u => u.Id == course.TeacherId);

            // Map the data to the CourseViewModel
            var viewModel = new CourseViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CourseCode = course.CourseCode,
                Credits = course.Credits,
                DepartmentName = course.Department?.DepartmentName,
                DepartmentId = course.DepartmentId,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                AddedDate = course.AddedDate,
                IsActive = course.IsActive,
                CourseImagePath = course.CourseImagePath,
                CreatorName = teacher != null ? $"{teacher.FirstName} {teacher.LastName}" : "Unknown"
            };

            return View(viewModel);
        }

        // GET: Courses/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "DepartmentName");
            return View();
        }

        private string GenerateCourseCode(string departmentCode, int currentCount)
        {
            return $"{departmentCode}-{currentCount:D3}";
        }

        private async Task<string?> UploadCourseImageAsync(IFormFile? imageFile)
        {
            if (imageFile is { Length: > 0 })
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images", "courses");
                Directory.CreateDirectory(uploadPath); // Ensure the directory exists
                var filePath = Path.Combine(uploadPath, fileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                return $"/uploads/images/courses/{fileName}";
            }
            return null;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel model, IFormFile? imageFile)
        {
            // Prepare Data
            var department = await _context.Departments.FindAsync(model.DepartmentId);
            var departmentCode = department?.DepartmentName.Substring(0, 2).ToUpper() ?? "XX";

            var latestCourse = _context.Courses
                .Where(c => c.DepartmentId == model.DepartmentId)
                .OrderByDescending(c => c.CourseCode)
                .FirstOrDefault();

            int courseCodeCounter = latestCourse != null && int.TryParse(latestCourse.CourseCode.Split('-').Last(), out int parsedCode)
                ? parsedCode + 1
                : 1;

            model.CourseCode = GenerateCourseCode(departmentCode, courseCodeCounter);
            model.CourseImagePath = await UploadCourseImageAsync(imageFile);

            // Validate Model
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
                ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName", model.DepartmentId);
                return View(model);
            }

            // Persist Data
            var course = new Course
            {
                Title = model.Title,
                Description = model.Description,
                DepartmentId = model.DepartmentId,
                Credits = model.Credits,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                IsActive = model.IsActive,
                CourseCode = model.CourseCode,
                CourseImagePath = model.CourseImagePath,
                TeacherId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                AddedDate = DateTime.UtcNow
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            // Redirect or Reset View
            ViewBag.Departments = new SelectList(_context.Departments, "Id", "DepartmentName");
            return RedirectToAction(nameof(Index)); // Redirect to the index page or another suitable action
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            // Map to ViewModel
            var model = new CourseViewModel
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                CourseCode = course.CourseCode,
                Credits = course.Credits,
                DepartmentId = course.DepartmentId,
                DepartmentName = course.Department.DepartmentName,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                AddedDate = course.AddedDate,
                IsActive = course.IsActive
            };

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "DepartmentName", course.DepartmentId);
            return View(model);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, CourseViewModel model, IFormFile? imageFile)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var course = await _context.Courses.FindAsync(id);
                    if (course == null) return NotFound();

                    // Update the properties
                    course.Title = model.Title;
                    course.Description = model.Description;
                    course.CourseCode = model.CourseCode;
                    course.Credits = model.Credits;
                    course.DepartmentId = model.DepartmentId;
                    course.StartDate = model.StartDate;
                    course.EndDate = model.EndDate;
                    course.IsActive = model.IsActive;

                    // Handle image update if applicable
                    if (imageFile is { Length: > 0 })
                    {
                        var fileName = Guid.NewGuid() + Path.GetExtension(imageFile.FileName);
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/images/courses");
                        Directory.CreateDirectory(uploadPath);
                        var filePath = Path.Combine(uploadPath, fileName);
                        await using var stream = new FileStream(filePath, FileMode.Create);
                        await imageFile.CopyToAsync(stream);
                        course.CourseImagePath = $"/uploads/images/courses/{fileName}";
                    }

                    _context.Update(course);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "DepartmentName", model.DepartmentId);
            return View(model);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool CourseExists(string id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
