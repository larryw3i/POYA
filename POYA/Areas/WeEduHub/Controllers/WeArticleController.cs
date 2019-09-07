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
            _weEduHubHelper=new WeEduHubHelper();
            _funFilesHelper=new FunFilesHelper();
            WeArticleControllerInitial();
        }
        #endregion

        // GET: WeEduHub/WeArticle
        public async Task<IActionResult> Index(Guid? SetId)
        {
            if(SetId==null)
            {
                return NotFound();
            }
            
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _WeArticle=await _context.WeArticle.Where(p=>p.UserId==_UserId && p.SetId==SetId).ToListAsync();
            
            ViewData[nameof(SetId)]=SetId;
            
            return View(_WeArticle);
        }

        // GET: WeEduHub/WeArticle/Details/5
        public async Task<IActionResult> Details(Guid? id)
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

            weArticle.Content=await System.IO.File.ReadAllTextAsync(  _weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv)+"/"+weArticle.WeArticleFileId  );

            return View(weArticle);
        }

        // GET: WeEduHub/WeArticle/Create
        public IActionResult Create(Guid? SetId)
        {
            if (SetId == null) return NotFound();

            var _WeArticle = new WeArticle { SetId = SetId ?? Guid.Empty, };

            return View(_WeArticle);
        }

        // POST: WeEduHub/WeArticle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SetId,Title,WeArticleFormFile")] WeArticle weArticle)
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

                weArticle.WeArticleFileId=_WeArticleFile.Id;

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

            var weArticle = await _context.WeArticle.FindAsync(id);
            if (weArticle == null)
            {
                return NotFound();
            }

            weArticle.Content=await System.IO.File.ReadAllTextAsync( _weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv)+"/"+weArticle.WeArticleFileId  );

            return View(weArticle);
        }

        // POST: WeEduHub/WeArticle/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,SetId,WeArticleFormFile")] WeArticle weArticle)
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
                    _WeArticle.Title=weArticle.Title;

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

                        _WeArticle.WeArticleFileId=_WeArticleFile.Id;
                    }
                    
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
                return RedirectToAction(nameof(Index),new{weArticle.SetId});
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

            weArticle.Content=await System.IO.File.ReadAllTextAsync(  _weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv)+"/"+weArticle.WeArticleFileId  );

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
        private void WeArticleControllerInitial()
        {
            if(!System.IO.Directory.Exists( _weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv))) 
                System.IO.Directory.CreateDirectory(_weEduHubHelper.WeEduHubFilesDirectoryPath(_hostingEnv) );
            
        }
        #endregion
    }
}
