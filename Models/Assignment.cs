using System;
using System.Collections.Generic;

namespace SciencesTechnology.Models;

public partial class Assignment
{
    public string Id { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public byte? MaxScore { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
