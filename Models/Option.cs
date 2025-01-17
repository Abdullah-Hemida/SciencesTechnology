using System;
using System.Collections.Generic;

namespace SciencesTechnology.Models;

public partial class Option
{
    public string Id { get; set; } = null!;

    public string? QuestionId { get; set; }

    public string? Text { get; set; }

    public bool IsCorrect { get; set; }

    public virtual QuizQuestion? Question { get; set; }
}
