namespace SciencesTechnology.ViewModels.Dashboard
{
    public class UserViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime DateOfRegistration { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public bool EmailConfirmed { get; set; }
        public string? ProfileImagePath { get; set; } // Updated type to string
        public int Credits { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? Address { get; set; }
    }
}
