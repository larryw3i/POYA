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
namespace POYA.Areas.Identity.Pages.Account
{
    public partial class CreateSuperUserModel : PageModel
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
        public CreateSuperUserModel(
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
        
        public bool IsEmailConfirmed { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        public string ReturnUrl { get; set; }
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }
            
            [Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }
            
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Required]
            [StringLength(256, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
            public string ConfirmPassword { get; set; }
            
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (ModelState.IsValid)
            {
                
                if(
                    await _context.Users.AnyAsync(p=>p.Email==Input.Email)    
                )
                {
                    ModelState.AddModelError( nameof(Input.Email), _localizer["Email is register"] );
                    return Page();
                }
                
                if(
                    ! await _roleManager.RoleExistsAsync(X_DOVEValues.SUPERUSER_String)
                )
                {
                    await _roleManager.CreateAsync(
                        new IdentityRole{
                            Name = X_DOVEValues.SUPERUSER_String,
                            NormalizedName = X_DOVEValues.SUPERUSER_String
                    });
                }

                var _SuperUser = new IdentityUser{
                        Email=Input.Email,
                        UserName = Input.UserName,
                        PhoneNumber=Input.PhoneNumber??"",
                        EmailConfirmed=true,
                    };

                await _userManager.CreateAsync(
                    _SuperUser,
                    Input.Password
                );

                await _userManager.AddToRoleAsync(
                    _SuperUser,
                    X_DOVEValues.SUPERUSER_String
                );

                return Redirect(returnUrl);
            }
            
            return Page();
        }
        
    }
}
