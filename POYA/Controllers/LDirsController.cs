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
        public async Task<IActionResult> Details(Guid? id)
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
            }   //  lDir.ReturnUrl = ReturnUrl ?? Url.Content("~/");
            lDir.InDirName= (await _context.LDir.Select(p => new { p.Id, p.Name }).FirstOrDefaultAsync(p => p.Id == lDir.InDirId))?.Name ?? "root";
            lDir.InFullPath = _x_DOVEHelper.GetFullPathOfFileOrDir(_context, lDir.InDirId);
            return View(lDir);  //   LocalRedirect(ReturnUrl); //  View(lDir);
        }

        // GET: LDirs/Create
        public async Task<IActionResult> Create(Guid? InDirId)
        {
            #region
            /*
            ViewData[nameof(InDirId)]= InDirId ?? Guid.Empty;
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var LDirs = await _context.LDir.Where(p => p.UserId == UserId_ && p.InDirId == InDirId).ToListAsync();
            */
            #endregion
            var LDir_ = new LDir
            {
                InDirId = InDirId ?? Guid.Empty,
                InDirName = (await _context.LDir.Where(p => p.InDirId == InDirId).Select(p => p.Name).FirstOrDefaultAsync()) ?? "root",
                //  ReturnUrl = ReturnUrl
            };
            return View(LDir_);
        }

        // POST: LDirs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,InDirId")] LDir lDir)
        {
            if (ModelState.IsValid)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                lDir.Id = Guid.NewGuid();
                lDir.UserId = UserId_;
                await _context.LDir.AddAsync(lDir);
                await _context.SaveChangesAsync();
                return RedirectToAction(actionName: "Index", controllerName: "LUserFiles", routeValues: new { lDir.InDirId });
                //  return LocalRedirect(lDir.ReturnUrl);
            }
            return View();
        }

        // GET: LDirs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lDir = await _context.LDir.FirstOrDefaultAsync(p=>p.UserId==UserId_ && p.Id==id);
            if (lDir == null)
            {
                return NotFound();
            }
            #region MOVE AND COPY

            var AllUserSubDirs = await _context.LDir.Where(p => p.Id != Guid.Empty && p.UserId == UserId_).ToListAsync();
            AllUserSubDirs.Remove(lDir);
            lDir.UserAllSubDirSelectListItems = new List<SelectListItem>();
            //  lUserFile.UserAllSubDirSelectListItems.Add(new SelectListItem {  Text="root/",Value=Guid.Empty.ToString()});
            lDir.UserAllSubDirSelectListItems.AddRange(AllUserSubDirs.Select(p => new SelectListItem { Text = $"{_x_DOVEHelper.GetFullPathOfFileOrDir(_context, p.InDirId)}{p.Name}", Value = p.Id.ToString() }).OrderBy(p => p.Text).ToList());
            lDir.CopyMoveSelectListItems = new List<SelectListItem>() {
                new SelectListItem{Text="Rename",Value="0",Selected=true},
                new SelectListItem{Text="Also copy", Value="1"}
            };
            #endregion

            return View(lDir);
        }

        // POST: LDirs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,DOCreate,InDirId,CopyMove")] LDir lDir)
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

                    if (!await _context.LDir.AnyAsync(p => p.Id == lDir.InDirId && p.UserId == UserId_))
                    {
                        ModelState.AddModelError(nameof(lDir.InDirId), "&#128557;");
                        return View(lDir);
                    }

                    #region COPY AND MOVE
                    //  Move
                    if (lDir.CopyMove == CopyMove.Move)
                    {
                        lDir.InDirId = lDir.InDirId;
                    }
                    /*
                    //  Copy
                    else if (lDir.CopyMove == CopyMove.Copy)
                    {
                        await _context.LDir.AddAsync(
                            new LDir
                            {
                                InDirId = lDir.InDirId,
                                //  MD5 = lDir.MD5,
                                Name =lDir.Name,
                                UserId = UserId_,
                                Id = Guid.NewGuid()
                            });
                    }
                    */
                    #endregion

                    else
                    {
                        _LDir.Name = lDir.Name;
                        //  _lUserFile.ContentType = _mimeHelper.GetMime(lUserFile.Name, _hostingEnv).Last();
                        //  _lUserFile.ContentType = _mimeHelper.GetMime(lUserFile.Name).Last();   // new Mime().Lookup(lUserFile.Name);
                        _context.Update(_LDir);
                        await _context.SaveChangesAsync();
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(actionName: "Index", controllerName: "LUserFiles", routeValues: new { _LDir.InDirId });
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
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lDir = await _context.LDir.FirstOrDefaultAsync(m => m.Id == id && m.UserId==UserId_);
            //  lDir.ReturnUrl = ReturnUrl ?? Url.Content("~/");
            if (lDir == null)
            {
                return NotFound();
            }
            lDir.InDirName = (await _context.LDir.Select(p => new { p.Id, p.Name }).FirstOrDefaultAsync(p => p.Id == lDir.InDirId))?.Name ?? "root";
            lDir.InFullPath = _x_DOVEHelper.GetFullPathOfFileOrDir(_context, lDir.InDirId);

            return View(lDir);
        }

        // POST: LDirs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lDir = await _context.LDir.FirstOrDefaultAsync(p=>p.Id==id && p.UserId==UserId_);
            if (lDir == null)
            {
                return NotFound();
            }
            var InDirId = lDir.InDirId;
            var _RemoveDirs = new List<LDir>() { lDir };
            #region ADD_ALL_INCLUDED_DIRS_TO_LIST
            var UserDirs = await _context.LDir.Where(p => p.UserId == UserId_).ToListAsync();
            foreach(var d in UserDirs)
            {
                var _InDirId = d.InDirId;
                while (_InDirId != Guid.Empty)
                {
                    if (_InDirId == null|| _InDirId==id || !UserDirs.Select(p => p.Id).Contains(_InDirId))
                    {
                        _RemoveDirs.Add(d);
                        break;
                    }
                    _InDirId = UserDirs.Where(p => p.Id == _InDirId).Select(p => p.InDirId).FirstOrDefault();
                    
                }
            }
            #endregion
            _context.LDir.RemoveRange(_RemoveDirs);
            await _context.SaveChangesAsync();
            return RedirectToAction(actionName:"Index",controllerName:"LUserFiles",routeValues:new { InDirId});  //   LocalRedirect(ReturnUrl??Url.Content("~/"));
        }

        private bool LDirExists(Guid id)
        {
            return _context.LDir.Any(e => e.Id == id);
        }

        #region DEPOLLUTION
        private bool IsFileOrDirInDir(List<LDir> UserDirs, Guid id, Guid DirId)
        {
            var IdArray = UserDirs.Select(p => p.Id);
            if (!IdArray.Contains(id)) return false;
            foreach (var d in UserDirs)
            {
                var _InDirId = d.InDirId;
                var _i = 0;
                while (_InDirId != Guid.Empty && _i < 30)
                {
                    if (_InDirId == DirId)
                    {
                        return true;
                    }
                    _InDirId = UserDirs.Where(p => p.Id == _InDirId).Select(p => p.InDirId).FirstOrDefault();
                    _i++;
                }
            }
            return false;
        }
        private List<LDir> GetAllSubDirs(List<LDir> UserDirs ,Guid DirId)
        {
            var SubDirs = new List<LDir>();
            foreach(var i in UserDirs) {
                if (IsFileOrDirInDir(UserDirs, i.Id, DirId))
                {
                    SubDirs.Add(i);
                }
            }
            return SubDirs;
        }
        #endregion
    }
}
