using System;
using System.Collections.Generic;

namespace SciencesTechnology.Models;

public partial class Lesson
{
    public string Id { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string? Title { get; set; }

    public string? Content { get; set; }

    public string? VideoUrl { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Course Course { get; set; } = null!;
}
