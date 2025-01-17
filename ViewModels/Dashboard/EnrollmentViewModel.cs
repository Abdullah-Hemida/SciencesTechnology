using SciencesTechnology.Models;

namespace SciencesTechnology.ViewModels.Dashboard
{
    public class EnrollmentViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StudentName { get; set; } = null!;
        public string StudentImagePath { get; set; } = "/images/default-student.jpg"; // Default image
        public string CourseTitle { get; set; } = null!;
        public DateTime EnrollmentDate { get; set; }
        public bool IsConfirmed { get; set; } = false;
        public Grade? Grade { get; set; }
    }
}

