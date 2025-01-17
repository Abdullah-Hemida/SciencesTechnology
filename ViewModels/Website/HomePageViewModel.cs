using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Dashboard;

namespace SciencesTechnology.ViewModels.Website
{
    public class HomePageViewModel
    {
        public AboutViewModel AboutSection { get; set; } = new AboutViewModel();
        public List<CourseViewModel> Courses { get; set; } = new List<CourseViewModel>();
        public List<UserViewModel> Teachers { get; set; } = new();
    }
}
