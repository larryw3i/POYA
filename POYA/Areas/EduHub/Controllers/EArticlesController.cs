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
using Microsoft.Extensions.Logging;
using POYA.Areas.EduHub.Models;
using POYA.Data;
using POYA.Unities.Helpers;
using X.PagedList;

namespace POYA.Areas.EduHub.Controllers
{
    [Area("EduHub")]
    [Authorize]
    public class EArticlesController : Controller
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
        private readonly ILogger<EArticlesController> _logger;
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly MimeHelper _mimeHelper;
        public EArticlesController(
            MimeHelper mimeHelper,
            HtmlSanitizer htmlSanitizer,
            ILogger<EArticlesController> logger,
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
            _mimeHelper = mimeHelper;
        }
        #endregion

        // GET: EduHub/EArticles
        public async Task<IActionResult> Index(bool? IsIndividual ,int _page = 1 )
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            #region CONTRAST IsIndividual
            if (IsIndividual == null)
            {
                    IsIndividual = (bool)(TempData[nameof(IsIndividual)] ?? false);
            }
            else
            {
                TempData[nameof(IsIndividual)] = IsIndividual;
            }
            #endregion

            var _EArticle = _context.EArticle.Where(p => IsIndividual==true ? (p.UserId == UserId_) : true)
               .OrderBy(p => p.DOPublishing);
            if (IsIndividual==false)
            {
                var _User = await _context.Users.Where(p=>p.EmailConfirmed).Select(p=>new { p.UserName,p.Id}).ToListAsync();
                await _EArticle.ForEachAsync(p=> {
                    p.UserName = _User.FirstOrDefault(o => o.Id == p.UserId)?.UserName;
                });
            }
            ViewData["EArticles"] = _EArticle.OrderByDescending(p=>p.DOPublishing).ToPagedList(_page, 8);
            ViewData[nameof(IsIndividual)] = IsIndividual;
            ViewData["UserId"] = UserId_; 
            return View();
        }

        // GET: EduHub/EArticles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var eArticle = await _context.EArticle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eArticle == null)
            {
                return NotFound();
            }

            return View(eArticle);
        }

        // GET: EduHub/EArticles/Create
        public async Task<IActionResult> Create()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _LUserFile = await _context.LUserFile.Where(p => p.UserId == UserId_  ).ToListAsync();  //<<<<<<<<
             
            var _EArticle = new EArticle { VideoSharedCodeSelectListItems = await GetVideoSharedCodeSelectListItemsForUser()};
            return View(_EArticle);
        }

        // POST: EduHub/EArticles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content")] EArticle eArticle)
        {
            if (ModelState.IsValid)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                eArticle.Id = Guid.NewGuid();
                eArticle.UserId = UserId_;
                eArticle.Content = _htmlSanitizer.Sanitize(eArticle.Content);
                _context.Add(eArticle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eArticle);
        }

        // GET: EduHub/EArticles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var eArticle = await _context.EArticle.FirstOrDefaultAsync(p=>p.Id==id && p.UserId==UserId_);
            if (eArticle == null)
            {
                return NotFound();
            }
             
            eArticle.VideoSharedCodeSelectListItems = await GetVideoSharedCodeSelectListItemsForUser();
            return View(eArticle);
        }

        // POST: EduHub/EArticles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,VideoSharedCode,Title,Content")] EArticle eArticle)
        {
            if (id != eArticle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    var _EArticle = await _context.EArticle.Where(p => p.Id == eArticle.Id && p.UserId == UserId_).FirstOrDefaultAsync();
                    #region UPDATE
                    _EArticle.Content =_htmlSanitizer.Sanitize( eArticle.Content);
                    _EArticle.DOUpdating = DateTimeOffset.Now;
                    _EArticle.VideoSharedCode = eArticle.VideoSharedCode;
                    _EArticle.Title = eArticle.Title;
                    #endregion
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EArticleExists(eArticle.Id))
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
            return View(eArticle);
        }

        // GET: EduHub/EArticles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var eArticle = await _context.EArticle
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId==UserId_);
            if (eArticle == null)
            {
                return NotFound();
            }

            return View(eArticle);
        }

        // POST: EduHub/EArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var eArticle = await _context.EArticle.FirstOrDefaultAsync(p=>p.Id==id && p.UserId==UserId_);
            _context.EArticle.Remove(eArticle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EArticleExists(Guid id)
        {
            return _context.EArticle.Any(e => e.Id == id);
        }


        #region DEPOLLUTION
        private async Task<List<SelectListItem>> GetVideoSharedCodeSelectListItemsForUser()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id; 

            var _LUserFile = await _context.LUserFile.Where(p => p.UserId == UserId_ ).ToListAsync();   //  <<<<<<<<
            var _VideoSharedCodeSelectListItems = new List<SelectListItem>() {
                new SelectListItem{  Value=Guid.Empty.ToString(), Text="Select your video file",Selected=true}
            };
            _LUserFile.ForEach(p => {
                if (_mimeHelper.GetMimes(p.Name.Split(".").LastOrDefault(), _hostingEnv).LastOrDefault().StartsWith("video")) { 
                _VideoSharedCodeSelectListItems.Add(
                    new SelectListItem { Text = _x_DOVEHelper.GetInPathOfFileOrDir(_context, p.InDirId) + p.Name, Value = p.Id.ToString() }   //<<<<<<<<
                    );
                }
            });
            return _VideoSharedCodeSelectListItems;
        }
        #endregion
    }
}
