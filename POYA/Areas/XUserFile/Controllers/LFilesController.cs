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
using POYA.Areas.XUserFile.Models;
using POYA.Data;
using POYA.Models;
using POYA.Unities.Helpers;

namespace POYA.Areas.XUserFile.Controllers
{

    [Area("LUserFile")]
    [Authorize]
    public class LFilesController : Controller
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
        private readonly ILogger<LFilesController> _logger;
        public LFilesController(
            ILogger<LFilesController> logger,
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
        // GET: LFiles
        public async Task<IActionResult> Index()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            return View(await _context.LFile.Where(p=>p.UserId==UserId_).ToListAsync());
        }

        // GET: LFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lFile = await _context.LFile
                .Where(p=>p.UserId==UserId_)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lFile == null)
            {
                return NotFound();
            }

            return View(lFile);
        }

        // GET: LFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,DOUpload,MD5")] LFile lFile)
        {
            if (ModelState.IsValid)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                lFile.Id = Guid.NewGuid();
                lFile.UserId = UserId_;
                _context.Add(lFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lFile);
        }

        #region BASIC FILE UNEDITED
        /*
        // GET: LFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lFile = await _context.LFile.FirstOrDefaultAsync(p=>p.UserId==UserId_ && p.Id==id);
            if (lFile == null)
            {
                return NotFound();
            }
            return View(lFile);
        }

        // POST: LFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,DOUpload,MD5")] LFile lFile)
        {
            if (id != lFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    var _lFile = await _context.LFile.FirstOrDefaultAsync(p => p.UserId == UserId_ && p.Id == lFile.Id);
                    if (_lFile == null)
                    {
                        return View(lFile);
                    }
                    //  _lFile
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LFileExists(lFile.Id))
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
            return View(lFile);
        }
        */
        #endregion

        // GET: LFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lFile = await _context.LFile
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId==UserId_);
            if (lFile == null)
            {
                return NotFound();
            }

            return View(lFile);
        }

        // POST: LFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lFile = await _context.LFile.FirstOrDefaultAsync(p=>p.UserId==UserId_ &&p.Id==id);
            if (lFile == null)
            {
                return NotFound();
            }
            _context.LFile.Remove(lFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LFileExists(Guid id)
        {
            return _context.LFile.Any(e => e.Id == id);
        }
    }
}
