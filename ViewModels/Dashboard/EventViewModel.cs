using System.ComponentModel.DataAnnotations;
using SciencesTechnology.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using SciencesTechnology.Enums;

namespace SciencesTechnology.ViewModels.Dashboard
{
    public class EventViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:MM}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:MM}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        public string? Location { get; set; }
        public string? EventImagePath { get; set; }

        [Display(Name = "Event Image")]
        public IFormFile? EventImage { get; set; }

        [Required]
        [Display(Name = "Organizer")]
        public string OrganizerId { get; set; } = null!;

        public List<OrganizerViewModel> Organizers { get; set; } = new();

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [Required]
        public EventType EventType { get; set; }
        public SelectList? EventTypes { get; set; }
        public bool IsOnline { get; set; }
        [Required]
        public int MaxParticipants { get; set; }
        public string CreatedBy { get; set; } = "System";
    }
    public class OrganizerViewModel
    {
        public string Id { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string? ProfileImagePath { get; set; } 
    }
}

