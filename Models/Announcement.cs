using System;
using System.Collections.Generic;

namespace SciencesTechnology.Models;

public partial class Announcement
{
    public string Id { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string? Title { get; set; }

    public string? Message { get; set; }

    public DateTime? PostedDate { get; set; }

    public virtual Course Course { get; set; } = null!;
}
