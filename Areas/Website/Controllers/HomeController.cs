using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciencesTechnology.Data;
using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Dashboard;
using SciencesTechnology.ViewModels.Website;
using WebsiteCourseVM = SciencesTechnology.ViewModels.Website.CourseViewModel;
using WebsiteAboutVM = SciencesTechnology.ViewModels.Website.AboutViewModel;
using System.Diagnostics;

namespace SciencesTechnology.Areas.Website.Controllers
{
    [Area("Website")]
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public HomeController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext)
        {
            _context = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var courseCount = await _context.Courses.CountAsync();
            var eventCount = await _context.Events.CountAsync();
            // Get roles
            var studentRole = await _roleManager.FindByNameAsync("Student");
            var teacherRole = await _roleManager.FindByNameAsync("Teacher");

            // Get users count for each role
            var studentCount = studentRole != null
                ? (await _userManager.GetUsersInRoleAsync("Student")).Count
            : 0;
            var teacherCount = teacherRole != null
                ? (await _userManager.GetUsersInRoleAsync("Teacher")).Count
                : 0;

            var About = new WebsiteAboutVM
            {
                StudentCount = studentCount,
                CourseCount = courseCount,
                EventCount = eventCount,
                TeacherCount = teacherCount
            };

            // Get the most enrolled courses (Top 3)
            var topCourses = await _context.Courses
                .OrderByDescending(c => c.Enrollments.Count) // Order by the number of enrollments
                .Take(3).Select(c => new WebsiteCourseVM
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    DepartmentName = c.Department.DepartmentName,
                    CourseCode = c.CourseCode,
                    Credits = c.Credits,
                    StartDate = c.StartDate,
                    EndDate = c.EndDate,
                    IsActive = c.IsActive,
                    CourseImagePath = c.CourseImagePath,
                    TeacherFirstName = c.Teacher.FirstName,
                    TeacherImagePath = c.Teacher.ProfileImagePath
                }).ToListAsync();
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            var teacherList = teachers.Select(t => new UserViewModel
            {
               UserId = t.Id,
               FirstName = t.FirstName,
               LastName = t.LastName,
               ProfileImagePath = t.ProfileImagePath,

            }).ToList();
            var viewModel = new HomePageViewModel
            {
                AboutSection = About,
                Courses = topCourses,
                Teachers = teacherList
            };
            ViewData["Title"] = "Home Page";
            return View(viewModel);
        }
    }
}