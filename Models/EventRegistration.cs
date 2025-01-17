using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SciencesTechnology.Models
{
    public partial class EventRegistration
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string EventId { get; set; } = string.Empty; // FK to Event

        public string UserId { get; set; } = string.Empty; // FK to ApplicationUser

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public string? Remarks { get; set; } // Optional field for additional information

        [ForeignKey("EventId")]
        public virtual Event Event { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } = null!;
    }

}
