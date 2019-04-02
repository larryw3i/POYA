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
    public class X_doveDirectoriesController : Controller
    {
        #region
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<Microsoft.AspNetCore.Identity.IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<HomeController> _logger;

        public X_doveDirectoriesController(
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
        /*
        // GET: X_doveDirectory
        public async Task<IActionResult> Index()
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            return View(await _context.X_doveDirectories.Where(p=>p.UserId==_UserId).ToListAsync());
        }

        // GET: X_doveDirectory/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            if (id == null)
            {
                return NotFound();
            }

            var x_doveDirectory = await _context.X_doveDirectory
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId==_UserId);
            if (x_doveDirectory == null)
            {
                return NotFound();
            }

            return View(x_doveDirectory);
        }
        */

        // GET: X_doveDirectory/Create
        public async Task<IActionResult> Create(string InDirId="")
        { 
            if (string.IsNullOrWhiteSpace(InDirId)) InDirId = Guid.Empty.ToString(); 
            TempData["InDirId"] = InDirId;
            var _InDirId = Guid.Parse(InDirId);
            ViewData["DirName"] =_InDirId==Guid.Empty?"./Root":(await _context.X_doveDirectories.Where(p => p.Id == _InDirId).Select(p => p.Name)
                .FirstOrDefaultAsync());
            return View();
        }

        // POST: X_doveDirectory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] X_doveDirectory x_doveDirectory)
        {
            var _InDirId = TempData["InDirId"].ToString();
            x_doveDirectory.InDirId = Guid.Parse(_InDirId);
            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                x_doveDirectory.Id = Guid.NewGuid();
                x_doveDirectory.UserId = _UserId;
                _context.Add(x_doveDirectory);
                await _context.SaveChangesAsync();
                return RedirectToAction(actionName: "Index", controllerName: "X_doveFiles", routeValues: new { DirId = _InDirId });
            }
            return View();
        }

        // GET: X_doveDirectory/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var x_doveDirectory = await _context.X_doveDirectories.Where(p => p.Id == id && p.UserId == _UserId).FirstOrDefaultAsync();    //  FindAsync(new { id,UserId=_UserId});
            if (x_doveDirectory == null)
            {
                return NotFound();
            }
            return View(x_doveDirectory);
        }

        // POST: X_doveDirectory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] X_doveDirectory x_doveDirectory)
        {
            if (id != x_doveDirectory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var _x_doveDirectory = await _context.X_doveDirectories.Where(p => p.Id == id && p.UserId==_UserId).FirstOrDefaultAsync();
                try
                {
                    _x_doveDirectory.Name = x_doveDirectory.Name; 
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!X_doveDirectoryExists(x_doveDirectory.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(actionName: "Index", controllerName: "X_doveFiles", routeValues: new { DirId = _x_doveDirectory.InDirId.ToString() });
            }
            return View(x_doveDirectory);
        }

        // GET: X_doveDirectory/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var x_doveDirectory = await _context.X_doveDirectories
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId==_UserId);
            if (x_doveDirectory == null)
            {
                return NotFound();
            }  
            return View(x_doveDirectory);
        }

        // POST: X_doveDirectory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var x_doveDirectory = await _context.X_doveDirectories.Where(p => p.Id == id && p.UserId == _UserId).FirstOrDefaultAsync();   //  .FindAsync(new { Id=id,UserId=_UserId});
            var DirId = x_doveDirectory.InDirId.ToString();
            _context.X_doveDirectories.Remove(x_doveDirectory);
            var _x_doveFiles = await _context.X_doveFiles.Where(p => p.CoopyOfId == Guid.Empty && p.InDirId == id).ToListAsync();
            _x_doveFiles.ForEach(p=>p.IsDeleted=true);
            _context.X_doveFiles.RemoveRange(_context.X_doveFiles.Where(p=>p.InDirId==id && p.CoopyOfId!=Guid.Empty));
            await _context.SaveChangesAsync();
            return RedirectToAction(actionName: "Index", controllerName: "X_doveFiles", routeValues: new { DirId });
        }

        private bool X_doveDirectoryExists(Guid id)
        {
            return _context.X_doveDirectories.Any(e => e.Id == id);
        }
    }
}
