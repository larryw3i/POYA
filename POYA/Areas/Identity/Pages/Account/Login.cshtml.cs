using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using POYA.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using POYA.Unities.Helpers;
using System.Text.Encodings.Web;
namespace POYA.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
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
        private readonly ILogger<ExternalLoginModel> _logger;
        public LoginModel(
            ILogger<ExternalLoginModel> logger,
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
            _logger = logger;
        }
        #endregion

        [BindProperty]
        public InputModel Input { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [Display(Name ="Password")]
            public string Password { get; set; }
            [Display(Name = "Remember me")]
            public bool RememberMe { get; set; }
        }
        public async Task OnGetAsync(string returnUrl = null, bool IsFromRegister = false, bool IsEmailConfirmed = true)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }
            if (IsFromRegister && !IsEmailConfirmed)
            {
                ModelState.AddModelError(nameof(Input.Email),_localizer["We have sent a confirmation email to you, you can login after confirming it (Didn't you get the email? check it in spam)"]);
            }
            else if (IsFromRegister) ModelState.AddModelError(nameof(Input.Email),_localizer[ "Your email is already registered in POYA, log in Now"]+" (^_^)");
            returnUrl = returnUrl ?? Url.Content("~/");
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var _user = await _userManager.FindByEmailAsync(Input.Email);
                if (_user==null)
                {
                    return RedirectToPage("Register",new { IsFromLogin = true, returnUrl });
                }
                if(! await _userManager.IsEmailConfirmedAsync(_user))
                {
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(_user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = _user.Id, code },
                        protocol: Request.Scheme);
                    await _emailSender.SendEmailAsync(Input.Email, _localizer["Confirm your email"],
                        $"{_localizer["Please confirm your account by"]} <a href='" + HtmlEncoder.Default.Encode(callbackUrl) + $"'>{_localizer["clicking here"]}</a>");
                    ModelState.AddModelError(string.Empty, _localizer["We have sent a confirmation email to you, you can login after confirming it"]);
                    return Page();
                }
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(
                    _userManager.FindByEmailAsync(Input.Email).GetAwaiter().GetResult().UserName, 
                    Input.Password, Input.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                }
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out");
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, _localizer[ "Invalid login attempt"]);
                    return Page();
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
