using System;
using System.Collections.Generic;

namespace SciencesTechnology.Models;

public partial class Submission
{
    public string Id { get; set; } = null!;

    public string AssignmentId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string? FileUrl { get; set; }

    public DateTime? SubmissionDate { get; set; }

    public byte? Score { get; set; }

    public virtual Assignment Assignment { get; set; } = null!;

}
