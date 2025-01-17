using System.Collections.Generic;

namespace SciencesTechnology.ViewModels.Dashboard
{
    public class UserRoleViewModel
    {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImagePath { get; set; } 
        public List<RoleViewModel>? Roles { get; set; } 
    }

    public class RoleViewModel
    {
        public string? RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}


