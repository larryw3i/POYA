using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using POYA.Areas.FunFiles.Controllers;
using POYA.Areas.WeEduHub.Models;
using POYA.Data;
using POYA.Unities.Helpers;
using X.PagedList;

namespace POYA.Areas.WeEduHub.Controllers
{
    [Authorize]
    [Area("WeEduHub")]
    public class WeArticleController : Controller
    {  
        #region     DI
        private readonly IHostingEnvironment _hostingEnv;
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
        public WeArticleController(
            HtmlSanitizer htmlSanitizer,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
           IEmailSender emailSender,
           UserManager<IdentityUser> userManager,
           ApplicationDbContext context,
           IHostingEnvironment hostingEnv,
           IStringLocalizer<Program> localizer)
        {
            _htmlSanitizer = htmlSanitizer;
            _hostingEnv = hostingEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
            _weEduHubArticleClassHelper=new WeEduHubArticleClassHelper(_hostingEnv );
            _weEduHubHelper=new WeEduHubHelper();
            _funFilesHelper=new FunFilesHelper();
            WeArticleControllerInitial();
        }
        #endregion

        // GET: WeEduHub/WeArticle
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            Guid? SetId,
            int? APage
        )
        {   
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult()?.Id;
            var _WeArticles=await _context.WeArticle
                .Where(
                    p=>
                        SetId==null?true:p.SetId==SetId
                )
                .ToListAsync();
            
            ViewData[nameof(SetId)]=SetId;

            ViewData["UserId"]=_UserId;

            ViewData["WeArticles"]=_WeArticles.ToPagedList(APage??1, 10);
            
            return View();
        }

        // GET: WeEduHub/WeArticle/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult()?.Id;

            var weArticle = await _context.WeArticle.Where(p=> p.Id==id).FirstOrDefaultAsync();

            if (weArticle == null)
            {
                return NotFound();
            }
            ViewData["UserId"]=_UserId;

            _weEduHubArticleClassHelper.InitialWeArticleClassName(ref weArticle,weArticle.ClassId);

            
            ViewData["WeArticleFileContentType"]=_funFilesHelper.GetContentType(
                await _context.WeArticleFile.Where(p=>p.Id==weArticle.WeArticleContentFileId).Select(p=>p.Name).FirstOrDefaultAsync()
            );

            return View(weArticle);
        }

        // GET: WeEduHub/WeArticle/Create
        public IActionResult Create(Guid? SetId)
        {
            if (SetId == null) return NotFound();

            var _WeArticle = new WeArticle 
                { 
                    SetId = SetId ?? Guid.Empty, 
                    ClassId=_weEduHubArticleClassHelper.GetAllSecondClasses().Select(p=>p.Id).FirstOrDefault()
                };

            return View(_WeArticle);
        }

        // POST: WeEduHub/WeArticle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SetId,Title,WeArticleFormFile,ClassId,CustomClass,Complex,Comment")] WeArticle weArticle)
        {
            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

                if(
                    !await _context.WeArticleSet.AnyAsync(p=>p.UserId==_UserId && p.Id==weArticle.SetId) ||
                    weArticle.WeArticleFormFile==null 
                )
                {
                    return NotFound();
                }

                weArticle.Id = Guid.NewGuid();
                weArticle.UserId=_UserId;

                var _WeArticleFile=new WeArticleFile{
                        DOUploading=DateTimeOffset.Now,
                        Id=Guid.NewGuid(),
                        Name=System.IO.Path.GetFileName(weArticle.WeArticleFormFile.FileName),
                        UserId=_UserId
                    };

                weArticle.WeArticleContentFileId=_WeArticleFile.Id;

                weArticle.DOPublishing=DateTimeOffset.Now;

                await _context.AddAsync(weArticle);
                await _context.AddAsync( _WeArticleFile );

                await System.IO.File.WriteAllBytesAsync(
                    _weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv)+"/"+_WeArticleFile.Id,
                    _funFilesHelper.GetFormFileBytes(weArticle.WeArticleFormFile)
                );

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),new{SetId=weArticle.SetId});
            }
            return View(weArticle);
        }

        // GET: WeEduHub/WeArticle/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var weArticle = await _context.WeArticle.Where(p=>p.UserId==_UserId && p.Id==id).FirstOrDefaultAsync();

            if (weArticle == null)
            {
                return NotFound();
            }

            ViewData["WeArticleFileContentType"]=_funFilesHelper.GetContentType(
                await _context.WeArticleFile.Where(p=>p.Id==weArticle.WeArticleContentFileId).Select(p=>p.Name).FirstOrDefaultAsync()
            );

            ViewData["FirstClassId"]=_weEduHubArticleClassHelper.GetFirstClassIdBySecondClassId(weArticle.ClassId);

            ViewData["IsEdit"]=true;


            return View("Create",weArticle);
        }

        // POST: WeEduHub/WeArticle/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,SetId,Title,WeArticleFormFile,ClassId,CustomClass,Complex,Comment")] WeArticle weArticle)
        {
            if (id != weArticle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    
                    var _WeArticle=await _context.WeArticle.Where(p=>p.Id==weArticle.Id && p.UserId==_UserId).FirstOrDefaultAsync();

                    if(weArticle.WeArticleFormFile!=null) 
                    {
                        var _WeArticleFile=new WeArticleFile{
                            DOUploading=DateTimeOffset.Now,
                            Id=Guid.NewGuid(),
                            Name=System.IO.Path.GetFileName(weArticle.WeArticleFormFile.FileName),
                            UserId=_UserId
                        };

                        await System.IO.File.WriteAllBytesAsync(
                            _weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv)+"/"+_WeArticleFile.Id,
                            _funFilesHelper.GetFormFileBytes(weArticle.WeArticleFormFile)
                        );

                        await _context.AddAsync(_WeArticleFile);
                        
                        _WeArticle.WeArticleContentFileId=_WeArticleFile.Id;
                    }
                    

                    _WeArticle.Title=weArticle.Title;
                    _WeArticle.Complex=weArticle.Complex;
                    _WeArticle.ClassId=weArticle.ClassId;
                    _WeArticle.CustomClass=weArticle.CustomClass;
                    _WeArticle.Comment=weArticle.Comment;
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeArticleExists(weArticle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new{SetId=weArticle.SetId});
            }
            return View(weArticle);
        }

        // GET: WeEduHub/WeArticle/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var weArticle = await _context.WeArticle.Where(p=>p.UserId==_UserId && p.Id==id).FirstOrDefaultAsync();
            if (weArticle == null)
            {
                return NotFound();
            }


            return View(weArticle);
        }

        // POST: WeEduHub/WeArticle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var weArticle = await _context.WeArticle.FindAsync(id);
            _context.WeArticle.Remove(weArticle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeArticleExists(Guid id)
        {
            return _context.WeArticle.Any(e => e.Id == id);
        }

        #region  DEPOLLUTION

        [AllowAnonymous]
        [ActionName("GetWeArticleFile")]
        public async Task<IActionResult> GetWeArticleFileAsync(Guid? Id)
        {
            if(Id==null)
            {
                return NotFound();
            }

            var _WeArticleFile=await _context.WeArticleFile.Where(p=>p.Id==Id).FirstOrDefaultAsync();

            if(_WeArticleFile==null)
            {
                return NotFound();
            }

            var _WeArticleFileBytes=await System.IO.File.ReadAllBytesAsync(_weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv)+"/"+_WeArticleFile.Id);

            return File(_WeArticleFileBytes,_funFilesHelper.GetContentType(_WeArticleFile.Name),true);
        }

        public IActionResult GetSecondClassesByFirstClassCode(string FirstClassCode)
        {
            var _SecondClasses=_weEduHubArticleClassHelper.GetSecondClassesByFirstClassCode(FirstClassCode);
            return View("FirstClassesSelect",_SecondClasses);
        }

        public IActionResult GetAllFirstClasses()
        {
            var _WeArticleFirstClasses=_weEduHubArticleClassHelper.GetAllFirstClasses();

            return View("FirstClassesSelect",_WeArticleFirstClasses);
        }

        private void WeArticleControllerInitial()
        {
            if(!System.IO.Directory.Exists( _weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv))) 
                System.IO.Directory.CreateDirectory(_weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv) );
            
        }
        #endregion
    }
}
