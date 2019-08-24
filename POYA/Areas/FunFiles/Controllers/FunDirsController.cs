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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using POYA.Areas.FunFiles.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.FunFiles.Controllers
{
    [Area("FunFiles")]    
    [Authorize]
    public class FunDirsController : Controller
    {
       
        #region DI
        private readonly IHostingEnvironment _hostingEnv;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly FunFilesHelper _funFilesHelper;
        public FunDirsController(
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IHostingEnvironment hostingEnv,
            IStringLocalizer<Program> localizer)
        {
            _configuration = configuration;
            _hostingEnv = hostingEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
            _funFilesHelper=new  FunFilesHelper();
        }

        #endregion

        // GET: FunFiles/FunDirs
        public async Task<IActionResult> Index(Guid? ParentDirId)
        {
            var User_=await _userManager.GetUserAsync(User);
            var _ParentDirId=ParentDirId??_funFilesHelper.RootDirId;
            var _FunDir=await _context.FunDir
                .Where(p=>p.UserId==User_.Id && p.ParentDirId==_ParentDirId)
                .ToListAsync();

            ViewData[nameof(ParentDirId)]=_ParentDirId;

            return View(_FunDir);
        }

        // GET: FunFiles/FunDirs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funDir = await _context.FunDir
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funDir == null)
            {
                return NotFound();
            }

            return View(funDir);
        }

        // GET: FunFiles/FunDirs/Create
        public IActionResult Create(Guid? ParentDirId)
        {
            var _FunDir=new FunDir
            {
                ParentDirId=ParentDirId??_funFilesHelper.RootDirId
            };
            return View(_FunDir);
        }

        // POST: FunFiles/FunDirs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentDirId,Name")] FunDir funDir)
        {
            if (ModelState.IsValid)
            {
                var User_=await _userManager.GetUserAsync(User);

                funDir.Id = Guid.NewGuid();
                funDir.DOCreating=DateTimeOffset.Now;
                funDir.UserId=User_.Id;

                _context.Add(funDir);
                await _context.SaveChangesAsync();
                return RedirectToAction(
                    nameof(Index),
                    new{ ParentDirId=funDir.ParentDirId }
                );
            }
            return View(funDir);
        }

        // GET: FunFiles/FunDirs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funDir = await _context.FunDir.FindAsync(id);
            if (funDir == null)
            {
                return NotFound();
            }
            return View(funDir);
        }

        // POST: FunFiles/FunDirs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ParentDirId,Name,UserId,DOCreating")] FunDir funDir)
        {
            if (id != funDir.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funDir);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunDirExists(funDir.Id))
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
            return View(funDir);
        }

        // GET: FunFiles/FunDirs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funDir = await _context.FunDir
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funDir == null)
            {
                return NotFound();
            }

            return View(funDir);
        }

        // POST: FunFiles/FunDirs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var funDir = await _context.FunDir.FindAsync(id);
            _context.FunDir.Remove(funDir);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FunDirExists(Guid id)
        {
            return _context.FunDir.Any(e => e.Id == id);
        }

        #region DEPOLLUTION

        public async Task<IActionResult> IndexFunYourFiles(Guid? ParentDirId)
        {
            var _ParentDirId=ParentDirId??_funFilesHelper.RootDirId;
            var User_=await _userManager.GetUserAsync(User);
            return View(await _context.FunYourFile
                .Where(p=>p.ParentDirId==_ParentDirId && p.UserId==User_.Id)
                .ToListAsync()
            );
        }

        public async Task<IActionResult> GetPathBreadcrumb(Guid? ParentDirId)
        {
            var _ParentDirId=ParentDirId??_funFilesHelper.RootDirId;

            var _FunDirs=new List<FunDir>();
            
            while(_ParentDirId!=_funFilesHelper.RootDirId){
                var _FunDir=await _context.FunDir.FirstOrDefaultAsync(p=>p.Id==_ParentDirId);
                if(_FunDir==null){
                    break;
                }
                _FunDirs.Add(_FunDir);
                _ParentDirId=_FunDir.ParentDirId;
            }

            return View("PathBreadcrumb",_FunDirs.OrderBy(p=>p.DOCreating));
        }

        #endregion
    }
}
