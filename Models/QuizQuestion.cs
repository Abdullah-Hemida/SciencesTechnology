using System;
using System.Collections.Generic;

namespace SciencesTechnology.Models;

public partial class QuizQuestion
{
    public string Id { get; set; } = null!;

    public string QuizId { get; set; } = null!;

    public string? QuestionText { get; set; }

    public byte? Marks { get; set; }

    public virtual ICollection<Option> Options { get; set; } = new List<Option>();

    public virtual Quiz Quiz { get; set; } = null!;
}
