using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Localization;
using POYA.Data;
using POYA.Unities.Helpers;
using POYA.Models;
using Microsoft.Extensions.Configuration;

namespace POYA.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ConfirmEmailModel : PageModel
    {
        #region
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        public ConfirmEmailModel(
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
           IEmailSender emailSender,
           UserManager<IdentityUser> userManager,
           ApplicationDbContext context,
           IWebHostEnvironment webHostEnv,
           IStringLocalizer<Program> localizer)
        {
            _webHostEnv = webHostEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
        }
        #endregion

        public async Task<IActionResult> OnGetAsync(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToPage("/Index");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Error confirming email for user with ID '{userId}':");
            }

            await InitializeAccountDataAsync( user);

            return Page();
        }


        #region 

        /// <summary>
        /// Initial account data for user
        /// </summary>
        /// <param name="context">The ApplicationDbContext</param>
        /// <param name="user">The User</param>
        /// <returns></returns>
        #endregion
        private async Task InitializeAccountDataAsync( IdentityUser user)
        {
            //  var context = ServiceLocator.Instance.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var _X_doveUserInfo = new X_doveUserInfo() { UserId = user.Id };
            await _context.X_DoveUserInfos.AddAsync(_X_doveUserInfo);
            //  await context.LUserMainSharedDirs.AddAsync(new LUserMainSharedDir {UserId=user.Id});
            await _context.SaveChangesAsync();
        }

    }

}
