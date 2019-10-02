
using System;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using POYA.Areas.FunAdmin.Controllers;
using POYA.Areas.FunFiles.Controllers;
using POYA.Areas.WeEduHub.Controllers;
using POYA.Areas.WeEduHub.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.WeEduHub.ViewComponents
{
     public class FunCommentsViewComponent : ViewComponent
    {
        
        #region     DI
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly WeEduHubHelper _weEduHubHelper;
        private readonly FunFilesHelper _funFilesHelper;
        private readonly WeEduHubArticleClassHelper _weEduHubArticleClassHelper;
        private readonly FunAdminHelper _funAdminHelper;
        public FunCommentsViewComponent(
            HtmlSanitizer htmlSanitizer,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnv,
            IStringLocalizer<Program> localizer)
        {
            _htmlSanitizer = htmlSanitizer;
            _webHostEnv = webHostEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
            _weEduHubArticleClassHelper=new WeEduHubArticleClassHelper(_webHostEnv );
            _weEduHubHelper=new WeEduHubHelper();
            _funFilesHelper=new FunFilesHelper();
            _funAdminHelper=new FunAdminHelper(_localizer,_context);
        }
        #endregion

        public async Task<IViewComponentResult> InvokeAsync(Guid WeArticleId, string ViewName = "Create")
        {
            var _UserId = _userManager.GetUserAsync(HttpContext.User).GetAwaiter().GetResult()?.Id;
            
            switch(ViewName){
                
                case "Create":
                    var _FunComment = new FunComment();
                    return View(ViewName, _FunComment);
                    
                case "Index":
                    var _FunComments = await _context.FunComment.Where(p=>p.WeArticleId==WeArticleId).ToListAsync();
                    return View(ViewName, _FunComments);
            }
            return Content(null);
        }

    }
}