
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SciencesTechnology.Models;
using SciencesTechnology.ViewModels.Dashboard;

namespace SciencesTechnology.Areas.Dashboard.Controllers
{
    [Area("Dashboard")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public AdminController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> ManageUserRoles()
        {
            var users = _userManager.Users.ToList();

            var model = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();

                model.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    ProfileImagePath = user.ProfileImagePath,
                    Roles = allRoles.Select(role => new RoleViewModel
                    {
                        RoleName = role,
                        IsSelected = userRoles.Contains(role)
                    }).ToList()
                });
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUserRoles(Dictionary<string, Dictionary<string, bool>> roles)
        {
            foreach (var userRoles in roles)
            {
                var userId = userRoles.Key;
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null) continue;

                var currentRoles = await _userManager.GetRolesAsync(user);

                foreach (var role in userRoles.Value)
                {
                    var roleName = role.Key;
                    var isSelected = role.Value;

                    if (isSelected && !currentRoles.Contains(roleName))
                    {
                        await _userManager.AddToRoleAsync(user, roleName);
                    }
                    else if (!isSelected && currentRoles.Contains(roleName))
                    {
                        await _userManager.RemoveFromRoleAsync(user, roleName);
                    }
                }
            }

            TempData["SuccessMessage"] = "User roles updated successfully.";
            return RedirectToAction("ManageUserRoles");
        }

        public IActionResult ManageRoles() => View(_roleManager.Roles);

        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName)) return RedirectToAction("ManageRoles");

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded ? "Role created successfully." : "Failed to create role.";
            return RedirectToAction("ManageRoles");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return RedirectToAction("ManageRoles");

            var result = await _roleManager.DeleteAsync(role);
            TempData[result.Succeeded ? "SuccessMessage" : "ErrorMessage"] = result.Succeeded ? "Role deleted successfully." : "Failed to delete role.";
            return RedirectToAction("ManageRoles");
        }
        public async Task<IActionResult> ViewUsers(string? roleName)
        {
            List<ApplicationUser> users;

            if (roleName == "Unconfirmed")
            {
                // Fetch only users with unconfirmed emails
                users = await _userManager.Users.Where(u => !u.EmailConfirmed).ToListAsync();
            }
            else if (roleName == "Unassigned")
            {
                // Fetch users not assigned to any role
                var allUsers = _userManager.Users.ToList();
                var allRoles = _roleManager.Roles.ToList();
                var usersInRoles = allRoles
                    .SelectMany(role => _userManager.GetUsersInRoleAsync(role.Name).Result)
                    .Distinct()
                    .ToList();
                users = allUsers.Except(usersInRoles).ToList();
            }
            else if (!string.IsNullOrEmpty(roleName))
            {
                // Fetch users in a specific role
                users = (await _userManager.GetUsersInRoleAsync(roleName)).ToList();
            }
            else
            {
                // Fetch all users
                users = await _userManager.Users.ToListAsync();
            }

            var model = users
                .OrderBy(u => u.DateOfRegistration)
                .Select(user => _mapper.Map<UserViewModel>(user))
                .ToList();

            ViewData["Title"] = string.IsNullOrEmpty(roleName) ? "All Users" : $"{roleName} Users";
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> ConfirmUserEmail(string UserId)
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(ViewUsers));
            }

            user.EmailConfirmed = true;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User email confirmed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "An error occurred while confirming the user's email.";
            }

            return RedirectToAction(nameof(ViewUsers), new { roleName = "Unconfirmed" });
        }


        public async Task<IActionResult> UserDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToAction("ViewUsers");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("ViewUsers");
            }

            var model = _mapper.Map<UserViewModel>(user);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "User ID is required.";
                return RedirectToAction(nameof(ViewUsers));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(ViewUsers));
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
            }

            return RedirectToAction(nameof(ViewUsers));
        }

    }
}



