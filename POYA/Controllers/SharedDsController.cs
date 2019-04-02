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
    //  If I earn money, be sure to give some to the IDE
    [Authorize]
    public class SharedDsController : Controller
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
        private readonly ILogger<HomeController> _logger;

        public SharedDsController(
            ILogger<HomeController> logger,
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

        /// <summary>
        /// GET: SharedDs
        /// #   Get the shared directory or sub-directories/sub-files in shared directory
        /// #   Index of the directories user share
        /// </summary>
        /// <param name="SharedDId">The id in SharedDs</param>
        /// <param name="SubFoDId">The id in X_doveDirectories or X_doveFiles</param>
        /// <returns>Return the IActionResult</returns>
        public async Task<IActionResult> Index(Guid? SharedDId, Guid? SubDId)
        {
            if (SharedDId != null)
            {
                var _SharedD_ = await _context.SharedDs.FirstOrDefaultAsync(p => p.Id == SharedDId);
                if (_SharedD_ == null)
                {
                    // Can't find anything
                    return View();
                }
                ViewData[nameof(SharedDId)] = SharedDId;
                _SharedD_.UserId_ = await _context.X_doveDirectories.Where(p => p.Id == _SharedD_.DirId).Select(p => p.UserId).FirstOrDefaultAsync();
                var LastDirId = Guid.Empty;
                if (SubDId != null)
                {

                    var _UserX_doveDirectories_ = await _context.X_doveDirectories.Where(p => p.UserId == _SharedD_.UserId_).Select(p => new DirSubFDId { FoDId = p.Id, InDirId = p.InDirId }).ToListAsync();
                    if(await _context.X_doveFiles.AnyAsync(p => p.Id == SubDId)){

                        var _UserX_doveFiles_ = await _context.X_doveFiles.Where(p => p.UserId == _SharedD_.UserId_).Select(p => new DirSubFDId { FoDId = p.Id, InDirId = p.InDirId }).ToListAsync();
                        if (IsSubFoDInDir(_UserX_doveDirectories_.Union(_UserX_doveFiles_), _SharedD_.DirId, SubDId ?? Guid.Empty))
                        {
                            var _x_doveFile = await _context.X_doveFiles.Where(p => p.Id == SubDId && !p.IsDeleted).Select(p => new { IsCopy = p.CoopyOfId != Guid.Empty, p.CoopyOfId, p.ContentType, p.Name }).FirstOrDefaultAsync();
                            if (_x_doveFile == null) return null;
                            var buffer = new byte[0];
                            if (_x_doveFile.IsCopy)
                                buffer = await _context.X_doveFiles.Where(p => p.Id == _x_doveFile.CoopyOfId).Select(p => p.FileBuffer).FirstOrDefaultAsync();
                            else
                                buffer = await _context.X_doveFiles.Where(p => p.Id == SubDId).Select(p => p.FileBuffer).FirstOrDefaultAsync();
                            return File(fileContents: buffer, contentType: _x_doveFile.ContentType, fileDownloadName: _x_doveFile.Name);
                        }
                        return View("GetSharedD");
                    }


                    if (SharedDId!=SubDId&& IsSubFoDInDir(_UserX_doveDirectories_, _SharedD_.DirId, SubDId ?? Guid.Empty))
                    {
                        var _GetSharedD = new GetSharedD();
                        LastDirId = _UserX_doveDirectories_.Where(p => p.FoDId == SubDId).Select(p => p.InDirId).FirstOrDefault();
                            //  await _context.X_doveDirectories.Where(o => o.Id == SubDId).Select(p => p.DirId).FirstOrDefaultAsync();
                        ViewData[nameof(LastDirId)]=LastDirId==SharedDId?Guid.Empty:LastDirId;
                        var _X_doveDirectories = await _context.X_doveDirectories.Where(p => p.InDirId == SubDId).ToListAsync();
                        var _X_doveFiles = await _context.X_doveFiles.Where(p => p.InDirId == SubDId).Select(p => new X_doveFile
                        {
                            #region
                            FileBuffer = null,
                            InDirId = p.InDirId,
                            ContentType = p.ContentType,
                            CoopyOfId = p.CoopyOfId,
                            DOUpload = p.DOUpload,
                            Hash = p.Hash,
                            Id = p.Id,
                            IsDeleted = p.IsDeleted,
                            IsLegal = p.IsLegal,
                            Name = p.Name,
                            Size = p.Size == 0 ? _context.X_doveFiles.Where(q => q.Id == p.CoopyOfId).Select(m => m.Size).FirstOrDefault() : p.Size,
                            UserId = p.UserId
                            #endregion
                        }).ToListAsync();
                        return View(viewName: "GetSharedD", model: new GetSharedD { X_DoveDirectories = _X_doveDirectories, X_DoveFiles = _X_doveFiles });
                    }
                    // Can't find anything
                    return View("GetSharedD");
                }
                var _X_doveDirectories_ = await _context.X_doveDirectories.Where(p => p.InDirId == _SharedD_.DirId).ToListAsync();
                var _X_doveFiles_ = await _context.X_doveFiles.Where(p => p.InDirId == _SharedD_.DirId).Select(p => new X_doveFile
                {
                    #region
                    FileBuffer = null,
                    InDirId = p.InDirId,
                    ContentType = p.ContentType,
                    CoopyOfId = p.CoopyOfId,
                    DOUpload = p.DOUpload,
                    Hash = p.Hash,
                    Id = p.Id,
                    IsDeleted = p.IsDeleted,
                    IsLegal = p.IsLegal,
                    Name = p.Name,
                    Size = p.Size,
                    UserId = p.UserId
                    #endregion
                }).ToListAsync();
                return View(viewName: "GetSharedD", model: new GetSharedD { X_DoveDirectories = _X_doveDirectories_, X_DoveFiles = _X_doveFiles_ });
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _X_doveDirectoryIds = await _context.X_doveDirectories.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
            var _SharedDs_ = await _context.SharedDs.Where(p => _X_doveDirectoryIds.Contains(p.DirId)&& 
                ( p.DOEnd == null || p.DOEnd < p.DOSharing ||p.DOEnd > DateTimeOffset.Now)).ToListAsync();
            _SharedDs_.ForEach(p =>
            {
                var _X_doveDirectory_ = _context.X_doveDirectories.Where(q => q.Id == p.DirId).Select(m => new { m.Name }).FirstOrDefaultAsync().GetAwaiter().GetResult();
                p.Name_ = _X_doveDirectory_.Name;
            });
            return PartialView(_SharedDs_);
        }

        private bool IsSubFoDInDir(IEnumerable<DirSubFDId> _DirSubFDIds ,Guid DirId, Guid SubFoDId)
        {
            var _LastDirId = SubFoDId;
            while (true)
            {
                if (_LastDirId == DirId)
                {
                    return true;
                }
                if (_LastDirId == Guid.Empty)
                {
                    break;
                }
                _LastDirId = _DirSubFDIds.Where(p => p.FoDId == _LastDirId).Select(p => p.InDirId).FirstOrDefault();

            }
            return false;
        }


        // GET: SharedDs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _X_doveDirectoryIds = await _context.X_doveDirectories.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
            var sharedD = await _context.SharedDs
                .FirstOrDefaultAsync(m => m.Id == id && _X_doveDirectoryIds.Contains(m.DirId));
            if (sharedD == null)
            {
                return NotFound();
            }

            return View(sharedD);
        }

        // GET: SharedDs/Create
        public async Task<IActionResult> Create(Guid id)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var _X_doveDirectory_ = await _context.X_doveDirectories.Where(p => p.UserId == _UserId && p.Id == id).FirstOrDefaultAsync();

            if (_X_doveDirectory_ == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["You are trying to attack! haha..."]);
                return View();
            }
            var _SharedD = new SharedD
            {
                DirId = id,
                Name_ = _X_doveDirectory_.Name
            };
            return View(_SharedD);
        }

        // POST: SharedDs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DirId,DOEnd,Tip")] SharedD sharedD)
        {
            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var _UserDirIds = await _context.X_doveDirectories.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
                if (!_UserDirIds.Contains(sharedD.DirId))
                {
                    ModelState.AddModelError(nameof(SharedD.DirId), _localizer["You are trying to attack! haha..."]);
                    return View();
                }
                sharedD.Id = Guid.NewGuid();
                _context.Add(sharedD);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sharedD);
        }

        // GET: SharedDs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var sharedD = await _context.SharedDs.FindAsync(id);
            if (sharedD == null)
            {
                return NotFound();
            }
            return View(sharedD);
        }

        // POST: SharedDs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("DirId,Id,DOSharing,DOEnd,Tip,UserId")] SharedD sharedD)
        {
            if (id != sharedD.Id)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sharedD);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SharedDExists(sharedD.Id))
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
            return View(sharedD);
        }

        // GET: SharedDs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var sharedD = await _context.SharedDs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sharedD == null)
            {
                return NotFound();
            }

            return View(sharedD);
        }

        // POST: SharedDs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _X_doveDirectoryIds = await _context.X_doveDirectories.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
            var sharedD = await _context.SharedDs.FirstOrDefaultAsync(p => p.Id == id && _X_doveDirectoryIds.Contains(p.DirId));
            _context.SharedDs.Remove(sharedD);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SharedDExists(Guid id)
        {
            return _context.SharedDs.Any(e => e.Id == id);
        }
    }
}
