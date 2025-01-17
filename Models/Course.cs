using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SciencesTechnology.Models
{
    public partial class Course
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        [RegularExpression(@"^[A-Za-z0-9\-]+$", ErrorMessage = "Course Code must be alphanumeric and can include dashes.")]
        public string CourseCode { get; set; } = string.Empty;

        [Range(0, 10, ErrorMessage = "Credits must be between 0 and 10.")]
        public int Credits { get; set; }

        [Required]
        public string TeacherId { get; set; } = string.Empty;

        [Required]
        public string DepartmentId { get; set; } = string.Empty;

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = false;

        public int? LikedNums { get; set; }

        public string? CourseImagePath { get; set; } 

        public virtual Department Department { get; set; } = null!;

        [ForeignKey("TeacherId")]
        public virtual ApplicationUser Teacher { get; set; } = null!;

        public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();
        public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public virtual ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }

}
