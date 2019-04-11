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
    public class LUserFilesController : Controller
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
        public LUserFilesController(
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
        }
        #endregion
        // GET: LUserFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.LUserFile.ToListAsync());
        }

        // GET: LUserFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lUserFile == null)
            {
                return NotFound();
            }

            return View(lUserFile);
        }

        // GET: LUserFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LUserFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MD5,UserId,SharedCode,DOGenerating,FileName")] LUserFile lUserFile)
        {
            if (ModelState.IsValid)
            {
                lUserFile.Id = Guid.NewGuid();
                _context.Add(lUserFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lUserFile);
        }

        // GET: LUserFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile.FindAsync(id);
            if (lUserFile == null)
            {
                return NotFound();
            }
            return View(lUserFile);
        }

        // POST: LUserFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,MD5,UserId,SharedCode,DOGenerating,FileName")] LUserFile lUserFile)
        {
            if (id != lUserFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lUserFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LUserFileExists(lUserFile.Id))
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
            return View(lUserFile);
        }

        // GET: LUserFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lUserFile == null)
            {
                return NotFound();
            }

            return View(lUserFile);
        }

        // POST: LUserFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lUserFile = await _context.LUserFile.FindAsync(id);
            _context.LUserFile.Remove(lUserFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult UploadFile(Guid? InDirId)
        {
            return View(new UploadFile { InDirId = InDirId ??Guid.Empty, Token = Guid.NewGuid() });
        }

        [HttpPost]
        public IActionResult UploadFile()
        {
            return Json(new { });
        }

        [HttpPost]
        public async Task<IActionResult> ContrastMD5(ContrastMD5 ContrastMD5_)
        {
            var UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var UploadedFileMD5s = new List<string>();
            foreach(var i in ContrastMD5_.FileMD5s_)
            {
                if(await _context.LFile.AnyAsync(p => p.MD5 == i.MD5))
                {
                    await _context.LUserFile.AddAsync(
                        new LUserFile {   UserId=UserId, Name=i.FileName, InDirId=ContrastMD5_.InDirId}
                        );
                    UploadedFileMD5s.Add(i.MD5);
                }
            }
            await _context.SaveChangesAsync();
            return Json(UploadedFileMD5s);
        }

        private bool LUserFileExists(Guid id)
        {
            return _context.LUserFile.Any(e => e.Id == id);
        }
    }
}
