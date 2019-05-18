using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class LSharingsController : Controller
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
        private readonly ILogger<LSharingsController> _logger;
        private readonly XUserFileHelper _xUserFileHelper = new XUserFileHelper();
        public LSharingsController(
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

        // GET: XUserFile/LSharings
        public async Task<IActionResult> Index()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            
            //  var _LSharings=await _context.LSharings.Where()
            return View(await _context.LSharings.ToListAsync());
        }

        // GET: XUserFile/LSharings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lSharing = await _context.LSharings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSharing == null)
            {
                return NotFound();
            }

            return View(lSharing);
        }

        // GET: XUserFile/LSharings/Create
        public async Task<IActionResult> Create(Guid? _id)
        {
            if (_id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _lUserFile = await _context.LUserFile.Where(p => p.Id == _id && p.UserId == UserId_).FirstOrDefaultAsync();
            var _LDir = await _context.LDir.Where(p => p.UserId == UserId_ && p.Id == _id).FirstOrDefaultAsync();
            if(_lUserFile==null && _LDir == null)
            {
                return NotFound();
            }
            ViewData[nameof(_lUserFile)] = _lUserFile;
            ViewData[nameof(_LDir)] = _LDir;
            var _lSharing = new LSharing { OrginalId = _id ?? Guid.NewGuid() };
            return View(_lSharing);
        }

        // POST: XUserFile/LSharings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrginalId,Comment")] LSharing lSharing)
        {
            if (ModelState.IsValid)
            {
                #region DETERMINE A SHARED FILE OR DIRECTORY IS BELONG TO CURRENT USER OR NOT
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

                var _AllUserFiles = await _context.LUserFile.Where(p => p.UserId == UserId_).ToListAsync();
                var _AllLDirs = await _context.LDir.Where(p => p.UserId == UserId_).ToListAsync();

                var _lUserFile =_AllUserFiles.Where(p => p.Id == lSharing.OrginalId && p.UserId == UserId_).FirstOrDefault();
                var _LDir = _AllLDirs.Where(p => p.UserId == UserId_ && p.Id == lSharing.OrginalId).FirstOrDefault();
                if (_lUserFile == null && _LDir == null)
                {
                    return NotFound();
                }
                #endregion

                #region MAKE SUB DIRECTORY COPY IF SHARE A DIRECTORY
                if (_LDir != null)
                {
                    var _SubFiles = _xUserFileHelper.GetAllSubFiles(_AllLDirs, _AllUserFiles, lSharing.OrginalId);
                    var _SubDirs = _xUserFileHelper.GetAllSubDirs(_AllLDirs, lSharing.OrginalId);
                    var _LSharings = new List<LSharing>();
                    _SubDirs.ForEach(p => {
                        _LSharings.Add(new LSharing {  OrginalId=p.Id});
                    });
                    _SubFiles.ForEach(p => {
                        _LSharings.Add(new LSharing {  OrginalId=p.Id});
                    });
                    await _context.LSharings.AddRangeAsync(_LSharings);
                }
                #endregion

                lSharing.Id = Guid.NewGuid();
                _context.Add(lSharing);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lSharing);
        }

        // GET: XUserFile/LSharings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lSharing = await _context.LSharings.FindAsync(id);
            if (lSharing == null)
            {
                return NotFound();
            }
            return View(lSharing);
        }

        // POST: XUserFile/LSharings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,OrginalId,Comment")] LSharing lSharing)
        {
            if (id != lSharing.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    _context.Update(lSharing);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LSharingExists(lSharing.Id))
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
            return View(lSharing);
        }

        // GET: XUserFile/LSharings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lSharing = await _context.LSharings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSharing == null)
            {
                return NotFound();
            }

            return View(lSharing);
        }

        // POST: XUserFile/LSharings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lSharing = await _context.LSharings.FindAsync(id);
            _context.LSharings.Remove(lSharing);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LSharingExists(Guid id)
        {
            return _context.LSharings.Any(e => e.Id == id);
        }

        #region DEPOLLUTION

        #endregion
    }
}
