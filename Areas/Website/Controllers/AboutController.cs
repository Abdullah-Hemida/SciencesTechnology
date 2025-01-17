using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciencesTechnology.Data;
using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Website;
using System.Data;

namespace SciencesTechnology.Areas.Website.Controllers
{
    [Area("Website")]
    public class AboutController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        public AboutController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ApplicationDbContext dbContext) {
            _context = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            
            var courseCount = await  _context.Courses.CountAsync();
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

            var model = new AboutViewModel
            {
                StudentCount = studentCount,
                CourseCount = courseCount,
                EventCount = eventCount,
                TeacherCount = teacherCount
            };

            return View(model);
        }

    }
}
