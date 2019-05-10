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
    public class LUserSharingFilesController : Controller
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
        private readonly ILogger<LUserFilesController> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly MimeHelper _mimeHelper;
        private readonly LUserFilesController _lUserFilesController;
        public LUserSharingFilesController(
            LUserFilesController lUserFilesController,
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
            _lUserFilesController = lUserFilesController;
        }
        #endregion


        // GET: XUserFile/LUserSharingFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.LUserSharingFile.ToListAsync());
        }

        // GET: XUserFile/LUserSharingFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserSharingFile = await _context.LUserSharingFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lUserSharingFile == null)
            {
                return NotFound();
            }

            return View(lUserSharingFile);
        }

        // GET: XUserFile/LUserSharingFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: XUserFile/LUserSharingFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileId,SharingCode,Comment")] LUserSharingFile lUserSharingFile)
        {
            if (ModelState.IsValid)
            {
                lUserSharingFile.Id = Guid.NewGuid();
                _context.Add(lUserSharingFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lUserSharingFile);
        }

        // GET: XUserFile/LUserSharingFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserSharingFile = await _context.LUserSharingFile.FindAsync(id);
            if (lUserSharingFile == null)
            {
                return NotFound();
            }
            return View(lUserSharingFile);
        }

        // POST: XUserFile/LUserSharingFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FileId,SharingCode,Comment")] LUserSharingFile lUserSharingFile)
        {
            if (id != lUserSharingFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lUserSharingFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LUserSharingFileExists(lUserSharingFile.Id))
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
            return View(lUserSharingFile);
        }

        // GET: XUserFile/LUserSharingFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserSharingFile = await _context.LUserSharingFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lUserSharingFile == null)
            {
                return NotFound();
            }

            return View(lUserSharingFile);
        }

        // POST: XUserFile/LUserSharingFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lUserSharingFile = await _context.LUserSharingFile.FindAsync(id);
            _context.LUserSharingFile.Remove(lUserSharingFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LUserSharingFileExists(Guid id)
        {
            return _context.LUserSharingFile.Any(e => e.Id == id);
        }
    }
}
