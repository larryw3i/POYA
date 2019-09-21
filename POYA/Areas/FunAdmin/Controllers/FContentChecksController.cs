using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using POYA.Areas.FunAdmin.Models;
using POYA.Areas.FunFiles.Controllers;
using POYA.Areas.WeEduHub.Controllers;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.FunAdmin.Controllers
{
    [Authorize]
    [Area("FunAdmin")]
    public class FContentChecksController : Controller
    {

        #region DI
        private readonly IHostingEnvironment _hostingEnv;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly FunAdminHelper _funAdminHelper;
        private readonly WeEduHubHelper _weEduHubHelper;
        private readonly FunFilesHelper _funFilesHelper;
        private readonly WeEduHubArticleClassHelper _weEduHubArticleClassHelper;
        public FContentChecksController(
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IHostingEnvironment hostingEnv,
            IStringLocalizer<Program> localizer)
        {
            _configuration = configuration;
            _hostingEnv = hostingEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
            _funAdminHelper=new  FunAdminHelper(_localizer,_context);
            _weEduHubHelper=new WeEduHubHelper();
            _weEduHubArticleClassHelper=new WeEduHubArticleClassHelper(_hostingEnv);
            _funFilesHelper=new FunFilesHelper();
        }

        #endregion


        // GET: FunAdmin/FContentChecks
        public async Task<IActionResult> Index()
        {
            var User_=await _userManager.GetUserAsync(User);
            var UserId_=User_.Id;

            var _IsAdmin = await _userManager.IsInRoleAsync(User_,X_DOVEValues._administrator);

            var _FContentCheck=await _context.FContentCheck
                .Where(p=>_IsAdmin?true:p.AppellantId==UserId_)
                .ToListAsync();

            _FContentCheck.ForEach(p=>{
                p.AppellantName=_userManager.FindByIdAsync(p.AppellantId)?.GetAwaiter().GetResult()?.UserName??"N/A";
                p.ReceptionistName=_userManager.FindByIdAsync(p.ReceptionistId)?.GetAwaiter().GetResult()?.UserName??(_localizer["Wait"]+"...");
                p.ContentTitle=GetContentTitleString(p.ContentId).GetAwaiter().GetResult();
            });

            ViewData["IsAdmin"]=_IsAdmin;
            
            ViewData["UserId"]=UserId_;

            return View(_FContentCheck);
        }

        // GET: FunAdmin/FContentChecks/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            var User_=await _userManager.GetUserAsync(User);

            var _IsAdmin = await _userManager.IsInRoleAsync(User_,X_DOVEValues._administrator);

            var fContentCheck = await _context.FContentCheck
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fContentCheck == null)
            {
                return NotFound();
            }

            if( fContentCheck.AppellantId!=User_.Id && !_IsAdmin)
            {
                return NotFound();
            }

            ViewData["IsAdmin"]=_IsAdmin;

            return View(fContentCheck);
        }

        // GET: FunAdmin/FContentChecks/Create
        public async Task<IActionResult> Create(Guid _ContentId)
        {
            
            var User_=await _userManager.GetUserAsync(User);
            var _IsCurrentUserRoleInAdmin=await _userManager.IsInRoleAsync(User_,X_DOVEValues._administrator);

            var _FContentCheck = new FContentCheck
            {
                ContentId = _ContentId,
                IllegalityTypeSelectListItems = _funAdminHelper.GetIllegalityTypeSelectListItems(),
                IsLegal=false,
            };

            ViewData["IsCurrentUserRoleInAdmin"] = _IsCurrentUserRoleInAdmin;
            ViewData["IsReportSubmittedByUser"]= !_IsCurrentUserRoleInAdmin;
            ViewData["IsEdit"]=false;

            return View(_FContentCheck);
        }

        // POST: FunAdmin/FContentChecks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContentId,AppellantComment,ReceptionistComment,IsLegal,IllegalityType")] FContentCheck fContentCheck)
        {
            if (ModelState.IsValid)
            {
                var User_=await _userManager.GetUserAsync(User);

                var _IsCurrentUserRoleInAdmin = await _userManager.IsInRoleAsync(User_,X_DOVEValues._administrator);
                
                fContentCheck.Id = Guid.NewGuid();

                if(_IsCurrentUserRoleInAdmin)
                {   
                    fContentCheck.DOHandling=DateTimeOffset.Now;
                    fContentCheck.ReceptionistId=User_.Id;
                }
                else
                {
                    fContentCheck.DOSubmitting=DateTimeOffset.Now;
                    fContentCheck.AppellantId=User_.Id;
                    fContentCheck.IsLegal=true;
                }

                if(
                    !_funAdminHelper.GetIllegalityTypeSelectListItems().Select(p=>p.Value).Contains(fContentCheck.IllegalityType)
                )
                {
                    fContentCheck.IllegalityType="110";
                }

                _context.Add(fContentCheck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fContentCheck);
        }

        // GET: FunAdmin/FContentChecks/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fContentCheck = await _context.FContentCheck.FindAsync(id);

            if (fContentCheck == null)
            {
                return NotFound();
            }

            var User_=await _userManager.GetUserAsync(User);

            var _IsCurrentUserRoleInAdmin = await _userManager.IsInRoleAsync(User_,X_DOVEValues._administrator);
            
            ViewData["IsCurrentUserRoleInAdmin"]=_IsCurrentUserRoleInAdmin;
            
            ViewData["IsReportSubmittedByUser"]= (fContentCheck.AppellantId?.Length??0)>0;

            fContentCheck.IllegalityTypeSelectListItems=_funAdminHelper.GetIllegalityTypeSelectListItems();
            fContentCheck.AppellantComment=
                (fContentCheck.IllegalityType=="110" && _IsCurrentUserRoleInAdmin)?
                    (fContentCheck.IllegalityTypeSelectListItems.Where(p=>p.Value==fContentCheck.IllegalityType).Select(p=>p.Text).FirstOrDefault()+" >> "+fContentCheck.AppellantComment):
                    fContentCheck.AppellantComment;

            ViewData["IsEdit"]=true;

            return View("Create",fContentCheck);
        }

        // POST: FunAdmin/FContentChecks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AppellantId,ReceptionistId,AppellantComment,ReceptionistComment,IsLegal,IllegalityType")] FContentCheck fContentCheck)
        {
            if (id != fContentCheck.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var User_=await _userManager.GetUserAsync(User);

                    var _IsCurrentUserRoleInAdmin = await _userManager.IsInRoleAsync(User_,X_DOVEValues._administrator);


                    var _FContentCheck=await _context.FContentCheck.FirstOrDefaultAsync(
                        p=>
                            p.Id==id 
                            && _IsCurrentUserRoleInAdmin?true:
                            (p.AppellantId==User_.Id)
                    );

                    if(_FContentCheck==null)
                    {
                        return NotFound();
                    }

                    var IsReportSubmittedByUser=string.IsNullOrEmpty(_FContentCheck.AppellantId);

                    var IsChecked=string.IsNullOrEmpty(_FContentCheck.ReceptionistId);


                    if(_IsCurrentUserRoleInAdmin)
                    {
                        _FContentCheck.ReceptionistComment=fContentCheck.ReceptionistComment;
                        
                        if(IsReportSubmittedByUser && IsChecked)
                        {
                            _FContentCheck.AppellantComment=AppellantCommentForSubsequentCheck(fContentCheck,_FContentCheck);
                        }

                        _FContentCheck.DOHandling=DateTimeOffset.Now;
                        _FContentCheck.ReceptionistId=User_.Id;
                        _FContentCheck.IsLegal=fContentCheck.IsLegal;
                    }
                    else
                    {
                        _FContentCheck.AppellantComment=fContentCheck.AppellantComment;
                        _FContentCheck.DOSubmitting=DateTimeOffset.Now;
                    }

                    _FContentCheck.IllegalityType=
                        (_IsCurrentUserRoleInAdmin && fContentCheck.IsLegal)?string.Empty:
                        _funAdminHelper.GetIllegalityTypeSelectListItems().Select(p=>p.Value).Contains(fContentCheck.IllegalityType)?
                        fContentCheck.IllegalityType:"110";


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FContentCheckExists(fContentCheck.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(fContentCheck);
        }


        // GET: FunAdmin/FContentChecks/Delete/5
        [Authorize(Roles="ADMINISTRATOR")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var User_=await _userManager.GetUserAsync(User);

            var _IsCurrentUserRoleInAdmin = await _userManager.IsInRoleAsync(User_,X_DOVEValues._administrator);

            var fContentCheck = await _context.FContentCheck.FirstOrDefaultAsync(p=>p.Id==id );

            if (fContentCheck == null)
            {
                return NotFound();
            }
            if(User_.Id!=fContentCheck.AppellantId && 
                ( fContentCheck.ReceptionistId!=User_.Id && 
                    _IsCurrentUserRoleInAdmin))
            {
                return NotFound();
            }

            var IsReportSubmittedByUser=!string.IsNullOrEmpty(fContentCheck.AppellantId);

            var IsChecked=!string.IsNullOrEmpty(fContentCheck.ReceptionistId);

            ViewData["IsCurrentUserRoleInAdmin"]=_IsCurrentUserRoleInAdmin;

            return View(fContentCheck);
        }

        // POST: FunAdmin/FContentChecks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="ADMINISTRATOR")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            
            var User_=await _userManager.GetUserAsync(User);
            
            var _IsCurrentUserRoleInAdmin = await _userManager.IsInRoleAsync(User_,X_DOVEValues._administrator);

            var fContentCheck = await _context.FContentCheck.FirstOrDefaultAsync(
                p=>
                    p.AppellantId==User_.Id || 
                    (
                        string.IsNullOrEmpty(p.AppellantId) && 
                        p.ReceptionistId==User_.Id && 
                        _IsCurrentUserRoleInAdmin
                    )
            );
            _context.FContentCheck.Remove(fContentCheck);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool FContentCheckExists(Guid id)
        {
            return _context.FContentCheck.Any(e => e.Id == id);
        }

        #region DEPOLLUTION


        private string AppellantCommentForSubsequentCheck(FContentCheck fContentCheck, FContentCheck _FContentCheck)
        {
            return (_FContentCheck.IllegalityType == fContentCheck.IllegalityType && _FContentCheck.IllegalityType == "110") ?

                _FContentCheck.AppellantComment :

                (_funAdminHelper.GetIllegalityTypeSelectListItems()
                    .Where(p => p.Value == _FContentCheck.IllegalityType)
                    .Select(p => p.Text)
                    .FirstOrDefault() + "-->"

                        + (_FContentCheck.AppellantComment.Contains("-->") ?

                            _FContentCheck.AppellantComment.Substring(
                                _FContentCheck.AppellantComment.LastIndexOf("-->") + 3
                            ) :

                            _FContentCheck.AppellantComment
                        )
                );
        }


        [ActionName("RedirectionByContentId")]
        public async Task<IActionResult> RedirectionByContentIdAsync(Guid ContentId)
        {
            if(
                await _context.WeArticle.AnyAsync(p=>p.Id==ContentId)
            )
            {
                return RedirectToAction("Details","WeArticle",new{area="WeEduHub",Id=ContentId});
            }
            return NotFound();
        }

        
        public async Task<IActionResult> GetContent(Guid ContentId)
        {
            var _EArticle=await _context.EArticle.FirstOrDefaultAsync(p=>p.Id==ContentId);
            var _WeArticle=await _context.WeArticle.Where(p=>p.Id==ContentId).FirstOrDefaultAsync();
            var _User = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if(_EArticle!=null)
            {
                return Content(_EArticle.Content);
            }
            else if(_WeArticle!=null)
            {
                ViewData["AuthorUserId"]=_User?.Id??string.Empty;;
                ViewData["AuthorUserEmail"]=_User?.Email??string.Empty;

                _weEduHubArticleClassHelper.InitialWeArticleClassName(ref _WeArticle,_WeArticle.ClassId);

                ViewData["WeArticleFileContentType"]=_funFilesHelper.GetContentType(
                    await _context.WeArticleFile.Where(p=>p.Id==_WeArticle.WeArticleContentFileId).Select(p=>p.Name).FirstOrDefaultAsync()
                );
                
                return View("WeArticleDetails",_WeArticle);
            }
            return NoContent();
        }
        
        public async Task<string> GetContentTitleString(Guid ContentId)
        {
            var _EArticle=await _context.EArticle.FirstOrDefaultAsync(p=>p.Id==ContentId);
            var _WeArticle=await _context.WeArticle.Where(p=>p.Id==ContentId).FirstOrDefaultAsync();
            var _User = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if(_EArticle != null)
            {
                return _EArticle.Title;
            }
            else if(_WeArticle!=null)
            {
                return _WeArticle.Title;
            }
                
            return string.Empty;
        }
        public async Task<IActionResult> GetContentTitle(Guid ContentId)
        {
            var _EArticle=await _context.EArticle.FirstOrDefaultAsync(p=>p.Id==ContentId);
            var _WeArticle=await _context.WeArticle.Where(p=>p.Id==ContentId).FirstOrDefaultAsync();
            var _User = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if(_EArticle != null)
            {
                return Content(_EArticle.Title);
            }
            else if(_WeArticle!=null)
            {
                return Content(_WeArticle.Title);
            }
                
            return NoContent();
        }

        public IActionResult GetUserName(string UserId) => Content(_userManager.FindByIdAsync(UserId).GetAwaiter().GetResult()?.UserName);

        #endregion

    }
}
