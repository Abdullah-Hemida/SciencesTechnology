using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SciencesTechnology.Models
{
    public partial class Department
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
         
        [Required]
        public string DepartmentName { get; set; } = null!;

        [MinLength(10)]
        [MaxLength(255)]
        public string? Description { get; set; }
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedDate { get; set; }
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}

