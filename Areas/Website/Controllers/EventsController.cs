using Microsoft.AspNetCore.Mvc;
using SciencesTechnology.Data;
using Microsoft.EntityFrameworkCore;
using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Website;

namespace SciencesTechnology.Areas.Website.Controllers
{
    [Area("Website")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public EventsController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();

            return View(events);
        }

        public async Task<IActionResult> Details(string id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem == null)
            {
                return NotFound();
            }
            return View(eventItem);
        }

    }
}
