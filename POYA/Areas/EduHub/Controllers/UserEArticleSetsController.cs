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

namespace POYA.Areas.EduHub.Controllers
{
    [Area("EduHub")]
    [Authorize]
    public class UserEArticleSetsController : Controller
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
        private readonly ILogger<UserEArticleSetsController> _logger;
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly MimeHelper _mimeHelper;
        public UserEArticleSetsController(
            MimeHelper mimeHelper,
            HtmlSanitizer htmlSanitizer,
            ILogger<UserEArticleSetsController> logger,
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
        /*
        private readonly ApplicationDbContext _context;

        public UserEArticleSetsController(ApplicationDbContext context)
        {
            _context = context;
        }
        */
        #endregion

        #region 
        // GET: EduHub/UserEArticleSets
        #endregion
        public async Task<IActionResult> Index()
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _UserEArticleSet = await _context.UserEArticleSet.Where(p => p.UserId == _UserId).ToListAsync();
            #region INITIALIZE_THE_DEFAULT
            _UserEArticleSet.Add(new UserEArticleSet { Label = _localizer["Click to add"], Name = _localizer["ADD"], Comment = _localizer["Click to add"] });
            _UserEArticleSet.Add(new UserEArticleSet {  Comment=_localizer["The default"], Name=_localizer["default"], Label=_localizer["default"]});
            #endregion
            ViewData[nameof(UserEArticleHomeInfo)] = (await _context.userEArticleHomeInfos.Where(p => p.UserId == _UserId).FirstOrDefaultAsync()) ?? new UserEArticleHomeInfo { UserId = _UserId, Comment = _localizer["No set yet"] +"!" };
            return View(_UserEArticleSet);
        }

        #region 
        // GET: EduHub/UserEArticleSets/Details/5
        #endregion
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var userEArticleSet = await _context.UserEArticleSet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userEArticleSet == null)
            {
                return NotFound();
            }

            return View(userEArticleSet);
        }

        #region 
        // GET: EduHub/UserEArticleSets/Create
        #endregion
        public IActionResult Create()
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            return View();
        }

        #region
        // POST: EduHub/UserEArticleSets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Name,Label,DOCreating,Comment")] UserEArticleSet userEArticleSet)
        {
            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                userEArticleSet.Id = Guid.NewGuid();
                _context.Add(userEArticleSet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userEArticleSet);
        }

        #region 
        // GET: EduHub/UserEArticleSets/Edit/5
        #endregion
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var userEArticleSet = await _context.UserEArticleSet.FindAsync(id);
            if (userEArticleSet == null)
            {
                return NotFound();
            }
            return View(userEArticleSet);
        }

        #region
        // POST: EduHub/UserEArticleSets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,Name,Label,DOCreating,Comment")] UserEArticleSet userEArticleSet)
        {
            if (id != userEArticleSet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    _context.Update(userEArticleSet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserEArticleSetExists(userEArticleSet.Id))
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
            return View(userEArticleSet);
        }

        #region 
        // GET: EduHub/UserEArticleSets/Delete/5
        #endregion
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var userEArticleSet = await _context.UserEArticleSet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userEArticleSet == null)
            {
                return NotFound();
            }

            return View(userEArticleSet);
        }

        #region 
        // POST: EduHub/UserEArticleSets/Delete/5
        #endregion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var userEArticleSet = await _context.UserEArticleSet.FindAsync(id);
            _context.UserEArticleSet.Remove(userEArticleSet);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserEArticleSetExists(Guid id)
        {
            return _context.UserEArticleSet.Any(e => e.Id == id);
        }
    }
}
