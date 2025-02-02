// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using SciencesTechnology.Models;
using SciencesTechnology.Services;
using SciencesTechnology.ViewModels.Website;

namespace SciencesTechnology.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ILocationService _locationService;

        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            ILocationService locationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _locationService = locationService;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ViewData["Title"] = "Register";
            var countries = await _locationService.GetCountriesAsync();

            // ✅ Extract country names
            var countryNames = countries
                .Where(c => c.Name != null)
                .Select(c => c.Name.Common)
                .ToList();

            ViewData["Countries"] = new SelectList(countryNames);           
        }

        public async Task<PartialViewResult> OnGetStatesAsync(string country)
        {
            if (string.IsNullOrEmpty(country))
            {
                return new PartialViewResult
                {
                    ViewName = "_StateOptions",
                    ViewData = new ViewDataDictionary<List<string>>(ViewData, new List<string>())
                };
            }

            var states = await _locationService.GetStatesByCountryAsync(country);

            return new PartialViewResult
            {
                ViewName = "_StateOptions",
                ViewData = new ViewDataDictionary<List<string>>(ViewData, states)
            };
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    PhoneNumber = Input.PhoneNumber,
                    Country = Input.Country,
                    State = Input.State,
                    Address = Input.Address,
                    DateOfRegistration = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow
                };

                // Handle Profile Image upload
                if (Input.ProfileImage != null)
                {
                    var uploadsFolder = Path.Combine("wwwroot","uploads","images", "profile");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}_{Input.ProfileImage.FileName}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await Input.ProfileImage.CopyToAsync(fileStream);
                    }

                    user.ProfileImagePath = $"/uploads/images/profile/{fileName}";
                }

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Reload countries if model validation fails
            var countries = await _locationService.GetCountriesAsync();

            // ✅ Extract country names
            var countryNames = countries
                .Where(c => c.Name != null)
                .Select(c => c.Name.Common)
                .ToList();

            ViewData["Countries"] = new SelectList(countryNames);

            return Page();
        }
    }
}
