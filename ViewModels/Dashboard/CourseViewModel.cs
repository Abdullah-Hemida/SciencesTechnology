using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SciencesTechnology.ViewModels.Dashboard
{
    public class CourseViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required, MaxLength(100)]
        public string? Title { get; set; }

        public string? Description { get; set; }

        public string CourseCode { get; set; } = string.Empty;

        [Range(0, 10, ErrorMessage = "Credits must be between 0 and 10.")]
        public int Credits { get; set; }

        public string? DepartmentName { get; set; }

        [Required]
        public string DepartmentId { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(6);

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AddedDate { get; set; }

        public bool IsActive { get; set; } = false;

        public int? LikedNums { get; set; }

        public IFormFile? CourseImage { get; set; }

        public string? CourseImagePath { get; set; }

        public string? CreatorName { get; set; }
    }
}



