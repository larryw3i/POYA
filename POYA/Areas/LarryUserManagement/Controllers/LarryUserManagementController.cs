

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using POYA.Areas.LarryUserManagement.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.LarryUserManagement.Controllers
{
    
    [Authorize(Roles = "SUPERUSER")]
    [Area("LarryUserManagement")]
    public class LarryUserManagementController : Controller
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
        private readonly IConfiguration _configuration;
        public LarryUserManagementController(
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
            _configuration = configuration;
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

        [ActionName("Create")]
        public  IActionResult UserCreate()
        {
            return View();
        }

        [ActionName("Index")]
        public async System.Threading.Tasks.Task<IActionResult> UserIndexAsync()
        {
            var _Users = await _context.Users.ToListAsync();
            

            return View("Index", _Users);
        }
        


    }
}