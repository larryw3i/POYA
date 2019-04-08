using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Encodings.Web;
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
 namespace POYA.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
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
         public IndexModel(
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
        // public string Username { get; set; }
        public bool IsEmailConfirmed { get; set; }
         [TempData]
         public string StatusMessage { get; set; }
         [BindProperty]
         public InputModel Input { get; set; }
         public class InputModel
         {
             [Required]
             [EmailAddress]
             [Display(Name = "Email")]
             public string Email { get; set; }
             [Phone]
             [Display(Name = "Phone number")]
             public string PhoneNumber { get; set; }
             [Display(Name = "User name")]
             public string UserName { get; set; }
         }
         public async Task<IActionResult> OnGetAsync()
         {
             var user = await _userManager.GetUserAsync(User);
             if (user == null)
             {
                 return NotFound($"{_localizer["Unable to load user with ID"]} '{_userManager.GetUserId(User)}'");
             }
             var userName = await _userManager.GetUserNameAsync(user);
             var email = await _userManager.GetEmailAsync(user);
             var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
             // Username = userName;
             Input = new InputModel
             {
                 Email = email,
                 PhoneNumber = phoneNumber,
                 UserName = userName
             };
             IsEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
             return Page();
         }
         public async Task<IActionResult> OnPostAsync()
         {
             if (!ModelState.IsValid)
             {
                 return Page();
             }
             var user = await _userManager.GetUserAsync(User);
             if (user == null)
             {
                 return NotFound($"{_localizer["Unable to load user with ID"]} '{_userManager.GetUserId(User)}'");
             }
             var email = await _userManager.GetEmailAsync(user);
             if (Input.Email != email)
             {
                 var setEmailResult = await _userManager.SetEmailAsync(user, Input.Email);
                 if (!setEmailResult.Succeeded)
                 {
                     var userId = await _userManager.GetUserIdAsync(user);
                     throw new InvalidOperationException($"{_localizer["Unexpected error occurred setting email for user with ID"]} '{userId}'");
                 }
             }
             var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
             if (Input.PhoneNumber != phoneNumber)
             {
                 var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                 if (!setPhoneResult.Succeeded)
                 {
                     var userId = await _userManager.GetUserIdAsync(user);
                     throw new InvalidOperationException($"{_localizer["Unexpected error occurred setting phone number for user with ID"]} '{userId}'");
                 }
             }
             var userName = await _userManager.GetUserNameAsync(user);
             if (Input.UserName != userName)
             {
                 var IsUserNameUsed = await _context.Users.AnyAsync(p => p.UserName == Input.UserName);
                 if (IsUserNameUsed)
                 {
                     ModelState.AddModelError(nameof(Input.UserName), _localizer["User name is used, make another choice"]);
                     return Page();
                 }
                 var setUserNameResult = await _userManager.SetUserNameAsync(user, Input.UserName);
                 if (!setUserNameResult.Succeeded)
                 {
                     var userId = await _userManager.GetUserIdAsync(user);
                     throw new InvalidOperationException($"{_localizer["Unexpected error occurred setting user name for user with ID"]} '{userId}'.");
                 }
             }
             await _signInManager.RefreshSignInAsync(user);
             StatusMessage = _localizer["Your profile has been updated"];
             return RedirectToPage();
         }
         public async Task<IActionResult> OnPostSendVerificationEmailAsync()
         {
             if (!ModelState.IsValid)
             {
                 return Page();
             }
             var user = await _userManager.GetUserAsync(User);
             if (user == null)
             {
                 return NotFound($"{_localizer["Unable to load user with ID"]} '{_userManager.GetUserId(User)}'");
             }
             var userId = await _userManager.GetUserIdAsync(user);
             var email = await _userManager.GetEmailAsync(user);
             var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
             var callbackUrl = Url.Page(
             "/Account/ConfirmEmail",
             pageHandler: null,
             values: new { userId = userId, code = code },
             protocol: Request.Scheme);
             await _emailSender.SendEmailAsync(
             email,
             _localizer["Confirm your email"],
             $"{_localizer["Please confirm your account by"]} <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>{_localizer["clicking here"]}</a>");
             StatusMessage = _localizer["Verification email sent. Please check your email"];
             return RedirectToPage();
         }
     }
}
