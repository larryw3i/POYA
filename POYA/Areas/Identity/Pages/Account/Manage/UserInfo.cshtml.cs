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
using POYA.Unities.Attributes;
using POYA.Unities.Helpers;
namespace POYA.Areas.Identity.Pages.Account.Manage
{
    public partial class UserInfoModel : PageModel
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
        public UserInfoModel(
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
        // public string Username { get; set; }
        public bool IsEmailConfirmed { get; set; }
        [TempData]
        public string StatusMessage { get; set; }
        [BindProperty]
        public X_doveUserInfo Input { get; set; }
        #region
        //public X_doveUserInfo
        /*InputModel
    {
        #region
        [DOBirth(IsValueNullable = true)]
        [Display(Name = "Birth date")]
        public DateTimeOffset? DOBirth { get; set; }
        [Display(Name = "Overt")]
        public bool IsDOBirthOvert { get; set; } = false;
        [Display(Name = "Hobby")]
        [StringLength(maximumLength: 50)]
        public string Hobby { get; set; }
        [Display(Name = "Overt")]
        public bool IsHobbyOvert { get; set; } = false;
        [Display(Name = "Your hometown")]
        [StringLength(maximumLength: 100)]
        public string OriginalAddress { get; set; }
        [Display(Name = "Overt")]
        public bool IsOriginalAddressOvert { get; set; } = false;
        [Display(Name = "Your address")]
        [StringLength(maximumLength: 100)]
        public string Address { get; set; }
        [Display(Name = "Overt")]
        public bool IsAddressOvert { get; set; } = false;
        [Display(Name = "Marital status")]
        [StringLength(maximumLength: 10)]
        public string MaritalStatus { get; set; }
        [Display(Name = "Overt")]
        public bool IsMaritalStatusOvert { get; set; }
        [Display(Name = "Graduated from")]
        [StringLength(maximumLength: 50)]
        public string GraduatedFrom { get; set; }
        [Display(Name = "Overt")]
        public bool IsGraduatedFromOvert { get; set; } = false;
        [Display(Name = "Occupation")]
        [StringLength(maximumLength: 50)]
        public string Occupation { get; set; }
        [Display(Name = "Overt")]
        public bool IsOccupationOvert { get; set; } = false;
        #endregion
    }
    */
        #endregion
        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"{_localizer["Unable to load user with ID"]} '{_userManager.GetUserId(User)}'");
            }
            var _X_doveUserInfo = await _context.X_DoveUserInfos.Select(
                p => new X_doveUserInfo
                {
                    #region
                    Address = p.Address,
                    //  AvatarBuffer = null,
                    DOBirth = p.DOBirth,
                    Hobby = p.Hobby,
                    IsAddressOvert = p.IsAddressOvert,
                    IsDOBirthOvert = p.IsDOBirthOvert,
                    IsHobbyOvert = p.IsHobbyOvert,
                    IsMaritalStatusOvert = p.IsMaritalStatusOvert,
                    IsOriginalAddressOvert = p.IsOriginalAddressOvert,
                    MaritalStatus = p.MaritalStatus,
                    OriginalAddress = p.OriginalAddress,
                    UserId = p.UserId,
                    IsOccupationOvert = p.IsOccupationOvert,
                    Occupation = p.Occupation,
                    IsGraduatedFromOvert = p.IsGraduatedFromOvert,
                    GraduatedFrom = p.GraduatedFrom
                    #endregion
                })
                .FirstOrDefaultAsync(p => p.UserId == user.Id);
            Input = _X_doveUserInfo == null ? new X_doveUserInfo() { } : new X_doveUserInfo
            {
                #region
                OriginalAddress = _X_doveUserInfo.OriginalAddress,
                MaritalStatus = _X_doveUserInfo.MaritalStatus,
                Address = _X_doveUserInfo.Address,
                DOBirth = _X_doveUserInfo.DOBirth,
                Hobby = _X_doveUserInfo.Hobby,
                IsAddressOvert = _X_doveUserInfo.IsAddressOvert,
                IsDOBirthOvert = _X_doveUserInfo.IsDOBirthOvert,
                IsHobbyOvert = _X_doveUserInfo.IsHobbyOvert,
                IsMaritalStatusOvert = _X_doveUserInfo.IsMaritalStatusOvert,
                IsOriginalAddressOvert = _X_doveUserInfo.IsOriginalAddressOvert,
                GraduatedFrom = _X_doveUserInfo.GraduatedFrom,
                IsGraduatedFromOvert = _X_doveUserInfo.IsGraduatedFromOvert,
                Occupation = _X_doveUserInfo.Occupation,
                IsOccupationOvert = _X_doveUserInfo.IsOccupationOvert
                #endregion
            };
            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            var user = await _userManager.GetUserAsync(User);
            var _X_doveUserInfo = await _context.X_DoveUserInfos.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (_X_doveUserInfo != null)
            {
                #region UPDATE X_DOVEUSERINFO
                _X_doveUserInfo.Address = Input.Address;
                _X_doveUserInfo.DOBirth = Input.DOBirth;
                _X_doveUserInfo.GraduatedFrom = Input.GraduatedFrom;
                _X_doveUserInfo.Hobby = Input.Hobby;
                _X_doveUserInfo.IsAddressOvert = Input.IsAddressOvert;
                _X_doveUserInfo.IsDOBirthOvert = Input.IsDOBirthOvert;
                _X_doveUserInfo.IsGraduatedFromOvert = Input.IsGraduatedFromOvert;
                _X_doveUserInfo.IsHobbyOvert = Input.IsHobbyOvert;
                _X_doveUserInfo.IsMaritalStatusOvert = Input.IsMaritalStatusOvert;
                _X_doveUserInfo.IsOccupationOvert = Input.IsOccupationOvert;
                _X_doveUserInfo.IsOriginalAddressOvert = Input.IsOriginalAddressOvert;
                _X_doveUserInfo.MaritalStatus = Input.MaritalStatus;
                _X_doveUserInfo.Occupation = Input.Occupation;
                _X_doveUserInfo.OriginalAddress = Input.OriginalAddress;
                #endregion
            }
            else
            {
                #region     NEW_X_DOVEUSERINFO
                var _X_doveUserInfo_ = new X_doveUserInfo
                {
                    Address = Input.Address,
                    DOBirth = Input.DOBirth,
                    GraduatedFrom = Input.GraduatedFrom,
                    Hobby = Input.Hobby,
                    IsAddressOvert = Input.IsAddressOvert,
                    IsDOBirthOvert = Input.IsDOBirthOvert,
                    IsGraduatedFromOvert = Input.IsGraduatedFromOvert,
                    IsHobbyOvert = Input.IsHobbyOvert,
                    IsMaritalStatusOvert = Input.IsMaritalStatusOvert,
                    IsOccupationOvert = Input.IsOccupationOvert,
                    IsOriginalAddressOvert = Input.IsOriginalAddressOvert,
                    MaritalStatus = Input.MaritalStatus,
                    Occupation = Input.Occupation,
                    OriginalAddress = Input.OriginalAddress,
                    UserId = user.Id,
                };
                #endregion
                await _context.X_DoveUserInfos.AddAsync(_X_doveUserInfo_);
            }
            await _context.SaveChangesAsync();
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = _localizer["Your profile has been updated"];
            return RedirectToPage();
        }
    }
}