using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using POYA.Data;
using POYA.Models;
using POYA.Unities.Helpers;

namespace POYA.Areas.Identity.Pages.Account
{
    public class ViewUserInfoModel : PageModel
    {
        #region
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;

        public ViewUserInfoModel(
        SignInManager<IdentityUser> signInManager,
        X_DOVEHelper x_DOVEHelper,
        RoleManager<IdentityRole> roleManager,
        IEmailSender emailSender,
        UserManager<IdentityUser> userManager,
        ApplicationDbContext context,
        IHostingEnvironment hostingEnv,
        IStringLocalizer<Program> localizer)
        {
            _hostingEnv = hostingEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
        }
        #endregion
        public async Task<IActionResult> OnGetAsync(string UserId)
        {
            if (UserId == null)
            {
                return NotFound();
            }
            var _User = await _context.Users.FirstOrDefaultAsync(p => p.Id == UserId);
            ViewData[nameof(_User)] = _User;
            ViewData[nameof(X_doveUserInfo)] = await _context.x_DoveUserInfos.Select(p => new X_doveUserInfo
            {
                #region
                Address = p.Address,
                AvatarBuffer = null,
                DOBirth = p.DOBirth,
                GraduatedFrom = p.GraduatedFrom,
                Hobby = p.Hobby,
                IsAddressOvert = p.IsAddressOvert,
                IsDOBirthOvert = p.IsDOBirthOvert,
                IsGraduatedFromOvert = p.IsGraduatedFromOvert,
                IsHobbyOvert = p.IsHobbyOvert,
                IsMaritalStatusOvert = p.IsMaritalStatusOvert,
                IsOccupationOvert = p.IsOccupationOvert,
                IsOriginalAddressOvert = p.IsOriginalAddressOvert,
                MaritalStatus = p.MaritalStatus,
                Occupation = p.Occupation,
                OriginalAddress = p.OriginalAddress,
                UserId = p.UserId
                #endregion
            }).FirstOrDefaultAsync(p => p.UserId == UserId);

            return Page();
        }
    }
}