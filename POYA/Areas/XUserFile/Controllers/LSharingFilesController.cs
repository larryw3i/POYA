using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
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
using POYA.Unities.Helpers;

namespace POYA.Areas.XUserFile.Controllers
{
    [Area("XUserFile")]
    [Authorize]
    public class LSharingFilesController : Controller
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
        private readonly ILogger<LSharingFilesController> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly MimeHelper _mimeHelper;
        public LSharingFilesController(
            MimeHelper mimeHelper,
            IAntiforgery antiforgery,
            ILogger<LUserFilesController> logger,
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
            _mimeHelper = mimeHelper;

        }
        #endregion


        // GET: XUserFile/LSharingFiles
        public async Task<IActionResult> Index()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _LUserFiles = await _context.LUserFile.Where(p => p.UserId == UserId_ && p.IsLegal).ToListAsync();
            var _LUserDirs = await _context.LDir.Where(p => p.UserId == UserId_ ).ToListAsync();
            var IDs = _LUserDirs.Select(p=>p.Id).Union(_LUserFiles.Select(p=>p.Id));
            var _LSharingFile = await _context.LSharingFile
                .Where(p => IDs.Contains(p.LUserFileOrDirId))
                .Select(p => new LSharingFile
                {
                    Comment = p.Comment,
                    Id = p.Id,
                    IsFile_ = _LUserFiles.Select(c => c.Id).Contains(p.LUserFileOrDirId),
                    LUserFileOrDirId = p.LUserFileOrDirId,
                    LUserFileOrDirName_ = _LUserFiles.FirstOrDefault(z => z.Id == p.LUserFileOrDirId).Name ?? _LUserDirs.FirstOrDefault(c => c.Id == p.LUserFileOrDirId).Name,
                }).ToListAsync();
            return View(_LSharingFile);
        }

        // GET: XUserFile/LSharingFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lSharingFile = await _context.LSharingFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSharingFile == null)
            {
                return NotFound();
            }

            return View(lSharingFile);
        }

        // GET: XUserFile/LSharingFiles/Create
        public async Task<IActionResult> Create(Guid _id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _LUserFile = await _context.LUserFile.FirstOrDefaultAsync(p => p.UserId == UserId_ && p.Id == _id);
            var _LSharingFile = new LSharingFile() { LUserFileOrDirId = _id};
            if (_LUserFile != null)
            {
                _LSharingFile.LUserFile_ = _LUserFile;
            }
            else
            {
                var _LDir = await _context.LDir.FirstOrDefaultAsync(p => p.UserId == UserId_ && p.Id == _id);
                if (_LDir == null)
                {
                    return NotFound();
                }
                _LSharingFile.LDir_ = _LDir;
            }
            return View(_LSharingFile);
        }

        // POST: XUserFile/LSharingFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LUserFileOrDirId,Comment")] LSharingFile lSharingFile)
        {
            if (ModelState.IsValid)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                _context.Add(lSharingFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lSharingFile);
        }

        // GET: XUserFile/LSharingFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lSharingFile = await _context.LSharingFile.FindAsync(id);
            if (lSharingFile == null)
            {
                return NotFound();
            }
            return View(lSharingFile);
        }

        // POST: XUserFile/LSharingFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,LUserFileOrDirId,Comment,IsLegal")] LSharingFile lSharingFile)
        {
            if (id != lSharingFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    _context.Update(lSharingFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LSharingFileExists(lSharingFile.Id))
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
            return View(lSharingFile);
        }

        // GET: XUserFile/LSharingFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lSharingFile = await _context.LSharingFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSharingFile == null)
            {
                return NotFound();
            }

            return View(lSharingFile);
        }

        // POST: XUserFile/LSharingFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lSharingFile = await _context.LSharingFile.FindAsync(id);
            _context.LSharingFile.Remove(lSharingFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LSharingFileExists(Guid id)
        {
            return _context.LSharingFile.Any(e => e.Id == id);
        }

        #region DEPOLLUTION
        

        #endregion
    }
}
