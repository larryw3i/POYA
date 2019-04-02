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
    public class SharedFsController : Controller
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

        public SharedFsController(
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

        // GET: SharedFs
        public async Task<IActionResult> Index(Guid? id)
        {
            if(id!=null){
                var _SharedF_=await _context.SharedFs
                .FirstOrDefaultAsync(p=>p.Id==id && (p.DOEnd>DateTimeOffset.Now || p.DOEnd==null || p.DOEnd<p.DOSharing));
                var _X_doveFile_=_context.X_doveFiles.Where(q=>q.Id==_SharedF_.FileId)
                    .Select(p=>new{p.Name,p.Size,p.CoopyOfId,p.ContentType})
                    .FirstOrDefaultAsync().GetAwaiter().GetResult();
                _SharedF_.Name_= _X_doveFile_.Name;
                if(_SharedF_!=null){
                    #region 
                    //  ViewData["SharingUserName"]=(await _context.Users.FirstOrDefaultAsync(p=>p.Id==_SharedF_.UserId)).UserName;
                    //  ViewData makes it less readable
                    #endregion
                    _SharedF_.UserId_= await _context.X_doveFiles.Where(p => p.Id == _SharedF_.FileId).Select(p => p.UserId).FirstOrDefaultAsync();
                    _SharedF_.ShringUserName_=(await _context.Users.FirstOrDefaultAsync(p=>p.Id==_SharedF_.UserId_)).UserName;
                    _SharedF_.IsGetSharing_=true;
                    _SharedF_.Size_= _X_doveFile_.CoopyOfId == Guid.Empty ? _X_doveFile_.Size : _context.X_doveFiles.Where(q => q.Id == _X_doveFile_.CoopyOfId).Select(m => m.Size).FirstOrDefaultAsync().GetAwaiter().GetResult();
                    _SharedF_.ContentType_=_X_doveFile_.ContentType;
                    return View(new List<SharedF>{_SharedF_});
                }
                return View();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _X_doveFileIds_ = await _context.X_doveFiles.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
            var _SharedFs=await _context.SharedFs
                .Where(p=>_X_doveFileIds_.Contains(p.FileId))
                .ToListAsync();
            _SharedFs.ForEach( p =>{
                var _X_doveFile_= _context.X_doveFiles.Where(q=>q.Id==p.FileId).Select(n=>new{n.Name,n.ContentType}).FirstOrDefaultAsync().GetAwaiter().GetResult();
                p.Name_= _X_doveFile_.Name;
                p.ContentType_=_X_doveFile_.ContentType;
                });
            return View(_SharedFs);
        }

        // GET: SharedFs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var _X_doveFileIds_ = await _context.X_doveFiles.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
            var sharedF = await _context.SharedFs
                .FirstOrDefaultAsync(m => m.Id == id &&_X_doveFileIds_.Contains(m.FileId));
            if (sharedF == null)
            {
                return NotFound();
            }

            return View(sharedF);
        }

        // GET: SharedFs/Create
        public async Task<IActionResult> Create(Guid id)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _X_doveFile_=await _context.X_doveFiles.Where(p=>p.UserId==_UserId && p.Id==id).Select(p=>new{p.Name}).FirstOrDefaultAsync();
            if(_X_doveFile_==null){
                    ModelState.AddModelError(string.Empty,_localizer["You are trying to attack! haha..."]);
                    return View();
                }
            var _sharedF_=new SharedF{
                 FileId=id,
                Name_ = _X_doveFile_.Name
            };
            return View(_sharedF_);
        }

        // POST: SharedFs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FileId,DOEnd,Tip")] SharedF sharedF)
        {
            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var _UserFileIds=await _context.X_doveFiles.Where(p=>p.UserId==_UserId).Select(p=>p.Id).ToListAsync();
                if(!_UserFileIds.Contains(sharedF.FileId) ){
                    ModelState.AddModelError(nameof(sharedF.FileId),_localizer["You are trying to attack! haha..."]);
                    return View();
                }
                sharedF.Id = Guid.NewGuid();
                _context.Add(sharedF);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sharedF);
        }

        // GET: SharedFs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _X_doveFileIds_ = await _context.X_doveFiles.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
            var sharedF = await _context.SharedFs.FirstOrDefaultAsync(p=>p.Id==id && _X_doveFileIds_.Contains(p.FileId));
            if (sharedF == null)
            {
                return NotFound();
            }
            return View(sharedF);
        }

        // POST: SharedFs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,DOEnd,Tip")] SharedF sharedF)
        {
            if (id != sharedF.Id)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            if (ModelState.IsValid)
            {
                try
                {
                    var _X_doveFileIds_ = await _context.X_doveFiles.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
                    var _SharedF=await _context.SharedFs.FirstOrDefaultAsync(p=>p.Id==id && _X_doveFileIds_.Contains(p.FileId));
                    _SharedF.DOEnd=sharedF.DOEnd;
                    _SharedF.Tip=sharedF.Tip;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SharedFExists(sharedF.Id))
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
            return View(sharedF);
        }

        // GET: SharedFs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var _X_doveFileIds_ = await _context.X_doveFiles.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
            var sharedF = await _context.SharedFs
                .FirstOrDefaultAsync(m => m.Id == id && _X_doveFileIds_.Contains(m.FileId));
            if (sharedF == null)
            {
                return NotFound();
            }

            return View(sharedF);
        }

        // POST: SharedFs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _X_doveFileIds_ = await _context.X_doveFiles.Where(p => p.UserId == _UserId).Select(p => p.Id).ToListAsync();
            var sharedF = await _context.SharedFs.FirstOrDefaultAsync(p=>_X_doveFileIds_.Contains(p.FileId) && p.Id==id);
            _context.SharedFs.Remove(sharedF);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> GetSharedFile(Guid id){
            if(id==null)
            {
                return NoContent();
            }
            var FileId =( await _context.SharedFs.FirstOrDefaultAsync(p=>p.Id==id)).FileId;
            var _x_doveFile = await _context.X_doveFiles.Where(p => p.Id == FileId  &&p.IsLegal ).Select(p => new { IsCopy = p.CoopyOfId != Guid.Empty, p.CoopyOfId, p.ContentType, p.Name }).FirstOrDefaultAsync();
            if (_x_doveFile == null){
                 return  NoContent();
            }
            var buffer = new byte[0];
            if (_x_doveFile.IsCopy)
                buffer = await _context.X_doveFiles.Where(p => p.Id == _x_doveFile.CoopyOfId).Select(p => p.FileBuffer).FirstOrDefaultAsync();
            else
                buffer = await _context.X_doveFiles.Where(p => p.Id == FileId).Select(p => p.FileBuffer).FirstOrDefaultAsync();
            return File(fileContents: buffer, contentType: _x_doveFile.ContentType, fileDownloadName: _x_doveFile.Name);
        }

        private bool SharedFExists(Guid id)
        {
            return _context.SharedFs.Any(e => e.Id == id);
        }
    }
}
