using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using POYA.Data;
using POYA.Unities.Helpers;
using POYA.Models;
using Microsoft.Extensions.Configuration;

namespace POYA.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {

        #region DI
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<ExternalLoginModel> _logger;
        private readonly IConfiguration _configuration;
        //  private readonly AppInitialization _appInitialization;
        public RegisterModel(
            //  AppInitialization appInitialization,
            IConfiguration configuration,
            ILogger<ExternalLoginModel> logger,
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
            _logger = logger;
            _configuration = configuration;
        }
        #endregion


        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
            public string ConfirmPassword { get; set; }
        }


        public void OnGet(string returnUrl = null, bool IsFromLogin = false)
        {
            if (IsFromLogin) ModelState.AddModelError(nameof(Input.Email), _localizer["Your e-mail is not registered in POYA yet, register it Now"] + " (^_^)");
            ReturnUrl = returnUrl;
        }


        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var _user = await _userManager.FindByEmailAsync(Input.Email);
                if (_user != null)
                {
                    TempData[nameof(Input.Email)] = Input.Email;
                    return RedirectToPage("Login", new { IsFromRegister = true, returnUrl });
                }

                var user = new IdentityUser { UserName = Input.Email, Email = Input.Email };

                var _CreateUserResult = await _userManager.CreateAsync(user, Input.Password);

                var _IsResultSucceeded = _CreateUserResult.Succeeded;

                if (_CreateUserResult.Succeeded && Input.Email == _configuration["Administration:AdminEmail"])
                {

                    if(!await _roleManager.RoleExistsAsync(X_DOVEValues._administrator))
                    {
                        await _roleManager.CreateAsync(new IdentityRole{ Name=X_DOVEValues._administrator, NormalizedName=X_DOVEValues._administrator });
                    }

                    var _AddToRoleResult=await _userManager.AddToRoleAsync(user, X_DOVEValues._administrator);
                    
                    _IsResultSucceeded = _CreateUserResult.Succeeded &&_AddToRoleResult.Succeeded;

                    if(!_IsResultSucceeded){

                        foreach (var error in _CreateUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }

                        foreach (var error in _AddToRoleResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return Page();

                    }
                }

                if (_IsResultSucceeded)
                {
                    _logger.LogInformation("User created a new account with password");
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code },
                        protocol: Request.Scheme);
                        
                    await _emailSender.SendEmailAsync(
                        Input.Email, 
                        _localizer["Confirm your email"],
                        _localizer["Please confirm your account by"]+"<a href='" + HtmlEncoder.Default.Encode(callbackUrl) + "'>"+_localizer["clicking here"]+"</a>");
                    
                    //  await _signInManager.SignInAsync(user, isPersistent: false);
                    //  ModelState.AddModelError(nameof(Input.Email),_localizer["We have sent a confirmation email to you, you can login after confirming it"]);

                    TempData[nameof(Input.Email)] = Input.Email;
                    return RedirectToPage("Login", new { IsFromRegister = true, returnUrl, IsEmailConfirmed = false });
                }

            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
