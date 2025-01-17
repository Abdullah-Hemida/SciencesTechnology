namespace SciencesTechnology.ViewModels.Website
{
    public class CourseDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int Credits { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public string? CourseImagePath { get; set; }
        public string? TeacherFirstName { get; set; }
        public string? TeacherImagePath { get; set; }
        public bool IsEnrolled { get; set; } = false; // Check if student is enrolled
        public bool IsConfirmed { get; set; } = false; // Check if enrollment is approved
    }
}
