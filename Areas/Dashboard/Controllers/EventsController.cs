using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SciencesTechnology.Data;
using SciencesTechnology.Enums;
using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Dashboard;

namespace SciencesTechnology.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = "Admin,Teacher")]
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EventsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            var events = await _context.Events
                .Include(e => e.Organizer)
                .ToListAsync();

            return View(events);
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null) return NotFound();

            return View(@event);
        }

        // GET: Events/Create
        public async Task<IActionResult> Create()
        {
            var organizers = await _userManager.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                             _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                .Select(u => new OrganizerViewModel
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    ProfileImagePath = u.ProfileImagePath
                })
                .ToListAsync();
            var eventType = new SelectList(Enum.GetValues(typeof(EventType)));
            var viewModel = new EventViewModel
            {
                EventTypes = eventType,
                Organizers = organizers,
                StartDate = DateTime.Now, // Set default value
                EndDate = DateTime.Now.AddHours(1) // Example: 1 hour after start time
            };


            return View(viewModel);
        }

        private async Task<string?> UploadEventImageAsync(IFormFile? EventImage)
        {
            if (EventImage is { Length: > 0 })
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(EventImage.FileName);
                var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "images", "events");
                Directory.CreateDirectory(uploadPath); // Ensure the directory exists
                var filePath = Path.Combine(uploadPath, fileName);
                await using var stream = new FileStream(filePath, FileMode.Create);
                await EventImage.CopyToAsync(stream);
                return $"/uploads/images/events/{fileName}";
            }
            return null;
        }
        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel viewModel, IFormFile? EventImage)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var currentUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == currentUserId);

            viewModel.EventImagePath = await UploadEventImageAsync(EventImage);

            if (ModelState.IsValid)
            {
                var @event = new Event
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    StartDate = viewModel.StartDate,
                    EndDate = viewModel.EndDate,
                    Location = viewModel.Location,
                    OrganizerId = viewModel.OrganizerId,
                    CreatedDate = viewModel.CreatedDate,
                    EventType = viewModel.EventType,
                    IsOnline = viewModel.IsOnline,
                    MaxParticipants = viewModel.MaxParticipants,
                    CreatedBy = currentUser?.FullName ?? "System",
                    EventImagePath = viewModel.EventImagePath
                };

                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns when validation fails
            viewModel.EventTypes = new SelectList(Enum.GetValues(typeof(EventType)));
            viewModel.Organizers = await _userManager.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                             _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                .Select(u => new OrganizerViewModel
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    ProfileImagePath = u.ProfileImagePath
                })
                .ToListAsync();

            return View(viewModel);
        }


        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.Include(e => e.Organizer).FirstOrDefaultAsync(e => e.Id == id);
            if (@event == null) return NotFound();

            var organizers = await _userManager.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                             _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                .Select(u => new OrganizerViewModel
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    ProfileImagePath = u.ProfileImagePath
                })
                .ToListAsync();

            var viewModel = new EventViewModel
            {
                Id = @event.Id,
                Title = @event.Title,
                Description = @event.Description,
                StartDate = @event.StartDate,
                EndDate = @event.EndDate,
                Location = @event.Location,
                OrganizerId = @event.OrganizerId,
                EventImagePath = @event.EventImagePath,
                IsOnline = @event.IsOnline,
                EventType = @event.EventType,
                MaxParticipants = @event.MaxParticipants,
                Organizers = organizers,
                EventTypes = new SelectList(Enum.GetValues(typeof(EventType)), @event.EventType)
            };

            return View(viewModel);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EventViewModel viewModel, IFormFile? EventImage)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var @event = await _context.Events.FindAsync(id);
                if (@event == null) return NotFound();

                @event.Title = viewModel.Title;
                @event.Description = viewModel.Description;
                @event.StartDate = viewModel.StartDate;
                @event.EndDate = viewModel.EndDate;
                @event.Location = viewModel.Location;
                @event.OrganizerId = viewModel.OrganizerId;
                @event.IsOnline = viewModel.IsOnline;
                @event.EventType = viewModel.EventType;
                @event.MaxParticipants = viewModel.MaxParticipants;

                if (EventImage != null)
                {
                    @event.EventImagePath = await UploadEventImageAsync(EventImage);
                }

                _context.Update(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns when validation fails
            viewModel.EventTypes = new SelectList(Enum.GetValues(typeof(EventType)), viewModel.EventType);
            viewModel.Organizers = await _userManager.Users
                .Where(u => _context.UserRoles.Any(ur => ur.UserId == u.Id &&
                             _context.Roles.Any(r => r.Id == ur.RoleId && r.Name == "Teacher")))
                .Select(u => new OrganizerViewModel
                {
                    Id = u.Id,
                    FullName = $"{u.FirstName} {u.LastName}",
                    ProfileImagePath = u.ProfileImagePath
                })
                .ToListAsync();

            return View(viewModel);
        }


        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();

            var @event = await _context.Events.Include(e => e.Organizer).FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null) return NotFound();

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

