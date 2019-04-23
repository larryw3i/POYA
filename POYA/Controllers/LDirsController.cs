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
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using POYA.Data;
using POYA.Models;
using POYA.Unities.Helpers;

namespace POYA.Controllers
{
    [Authorize]
    public class LDirsController : Controller
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
        private readonly ILogger<LDirsController> _logger;
        public LDirsController(
            ILogger<LDirsController> logger,
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

        // GET: LDirs
        public async Task<IActionResult> Index()
        {
            return View(await _context.LDir.ToListAsync());
        }

        // GET: LDirs/Details/5
        public async Task<IActionResult> Details(Guid? id,string ReturnUrl=null)
        {
            if (id == null)
            {
                return NotFound();
            }

            //  ReturnUrl = ReturnUrl ?? Url.Content("~/");
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lDir = await _context.LDir.FirstOrDefaultAsync(m => m.Id == id &&m.UserId==UserId_);
            if (lDir == null)
            {
                return NotFound();
            }
            lDir.ReturnUrl = ReturnUrl ?? Url.Content("~/"); ;
            return View(lDir);  //   LocalRedirect(ReturnUrl); //  View(lDir);
        }

        // GET: LDirs/Create
        public async Task<IActionResult> Create(Guid? InDirId,string ReturnUrl=null)
        {
            #region
            /*
            ViewData[nameof(InDirId)]= InDirId ?? Guid.Empty;
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var LDirs = await _context.LDir.Where(p => p.UserId == UserId_ && p.InDirId == InDirId).ToListAsync();
            */
            #endregion
            ReturnUrl = ReturnUrl ?? Url.Content("~/");
            var LDir_ = new LDir
            {
                InDirId = InDirId ?? Guid.Empty,
                InDirName = (await _context.LDir.Where(p => p.InDirId == InDirId).Select(p => p.Name).FirstOrDefaultAsync()) ?? "root",
                ReturnUrl = ReturnUrl
            };
            return View(LDir_);
        }

        // POST: LDirs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,DOCreate,InDirId,ReturnUrl")] LDir lDir)
        {
            if (ModelState.IsValid)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                lDir.ReturnUrl = lDir.ReturnUrl ?? Url.Content("~/");
                lDir.Id = Guid.NewGuid();
                lDir.UserId = UserId_;
                await _context.LDir.AddAsync(lDir);
                await _context.SaveChangesAsync();
                return LocalRedirect(lDir.ReturnUrl);
            }
            return View();
        }

        // GET: LDirs/Edit/5
        public async Task<IActionResult> Edit(Guid? id, string ReturnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lDir = await _context.LDir.Select(p=>new LDir { Id=p.Id, Name=p.Name,ReturnUrl=ReturnUrl ,UserId=p.UserId,InDirId=p.InDirId})
                .FirstOrDefaultAsync(p=>p.UserId==UserId_ && p.Id==id);
            if (lDir == null)
            {
                return NotFound();
            }
            //  lDir.ReturnUrl = ReturnUrl ?? Url.Content("~/");
            return View(lDir);
        }

        // POST: LDirs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,DOCreate,InDirId,ReturnUrl")] LDir lDir)
        {
            if (id != lDir.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    var _LDir = await _context.LDir.FirstOrDefaultAsync(p => p.Id == lDir.Id && p.UserId == UserId_);
                    if(_LDir==null)
                    {
                        return NotFound();
                    }
                    _LDir.Name =lDir.Name;
                    //  _context.Update(lDir);
                    await _context.SaveChangesAsync();
                    return LocalRedirect(lDir.ReturnUrl ?? Url.Content("~/"));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LDirExists(lDir.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(lDir);
        }

        // GET: LDirs/Delete/5
        public async Task<IActionResult> Delete(Guid? id,string ReturnUrl=null)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lDir = await _context.LDir.FirstOrDefaultAsync(m => m.Id == id && m.UserId==UserId_);
            lDir.ReturnUrl = ReturnUrl ?? Url.Content("~/");
            if (lDir == null)
            {
                return NotFound();
            }

            return View(lDir);
        }

        // POST: LDirs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id, string ReturnUrl = null)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lDir = await _context.LDir.FirstOrDefaultAsync(p=>p.Id==id && p.UserId==UserId_);
            if (lDir == null)
            {
                return NotFound();
            }
            var InDirId = lDir.InDirId;
            _context.LDir.Remove(lDir);
            await _context.SaveChangesAsync();
            return LocalRedirect(ReturnUrl??Url.Content("~/"));
        }

        private bool LDirExists(Guid id)
        {
            return _context.LDir.Any(e => e.Id == id);
        }
    }
}
