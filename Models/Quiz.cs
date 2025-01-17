using System;
using System.Collections.Generic;

namespace SciencesTechnology.Models;

public partial class Quiz
{
    public string Id { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public string? Title { get; set; }

    public string? Description { get; set; }

    public byte? TotalMarks { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<QuizQuestion> QuizQuestions { get; set; } = new List<QuizQuestion>();
}
