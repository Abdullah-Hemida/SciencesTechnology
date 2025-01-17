using System.ComponentModel.DataAnnotations;
using SciencesTechnology.Enums;
namespace SciencesTechnology.Models
{
    public class Event
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
        public string? Location { get; set; } // Physical address or online meeting link
        [Required]
        public string OrganizerId { get; set; } = null!;
        public ApplicationUser Organizer { get; set; } = null!;

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = null!;
        public bool IsOnline { get; set; } = false;
        public string? EventImagePath { get; set; }
        [Required]
        public EventType EventType { get; set; }
        public int MaxParticipants { get; set; }
        public int CurrentParticipants => EventRegistrations?.Count ?? 0;
        public ICollection<EventRegistration>? EventRegistrations { get; set; } = new List<EventRegistration>();
    }
}

