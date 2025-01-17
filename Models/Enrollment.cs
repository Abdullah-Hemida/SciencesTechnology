using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SciencesTechnology.Models;

public partial class Enrollment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    public string CourseId { get; set; } = string.Empty;

    [Required]
    public string StudentId { get; set; } = string.Empty;

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

    public DateTime? CompletionDate { get; set; }
    public bool IsConfirmed { get; set; } = false;
    public Grade? Grade { get; set; } 

    [ForeignKey("StudentId")]
    public virtual ApplicationUser Student { get; set; } = null!;

    
    public virtual Course Course { get; set; } = null!;

}

public enum Grade
{
    A,
    B,
    C,
    D,
    F,
    Incomplete
}

