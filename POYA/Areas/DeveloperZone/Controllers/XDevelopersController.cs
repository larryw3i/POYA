using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using POYA.Areas.DeveloperZone.Models;
using POYA.Areas.XUserFile.Controllers;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.DeveloperZone.Controllers
{
    [Area("DeveloperZone")]
    public class XDevelopersController : Controller
    {
        #region     DI
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<Program> _logger;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly IAntiforgery _antiforgery;
        private readonly MimeHelper _mimeHelper;
        private readonly XUserFileHelper _xUserFileHelper;
        private readonly IConfiguration _configuration;
        public XDevelopersController(
            IConfiguration configuration,
            MimeHelper mimeHelper,
            IAntiforgery antiforgery,
            ILogger<Program> logger,
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
            _mimeHelper = mimeHelper;
            _xUserFileHelper = new XUserFileHelper();

        }
        #endregion


        #region 

        // GET: DeveloperZone/XDevelopers
        #endregion
        public async Task<IActionResult> Index()
        {
            return View(await _context.XDeveloper.ToListAsync());
        }

        #region 

        // GET: DeveloperZone/XDevelopers/Details/5
        #endregion
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloper = await _context.XDeveloper
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xDeveloper == null)
            {
                return NotFound();
            }

            return View(xDeveloper);
        }

        #region 

        // GET: DeveloperZone/XDevelopers/Create
        #endregion
        public IActionResult Create()
        {
            return View();
        }

        #region 

        // POST: DeveloperZone/XDevelopers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,HomeCoverImgMD5,DOJoining")] XDeveloper xDeveloper)
        {
            if (ModelState.IsValid)
            {
                xDeveloper.Id = Guid.NewGuid();
                _context.Add(xDeveloper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(xDeveloper);
        }

        #region 

        // GET: DeveloperZone/XDevelopers/Edit/5
        #endregion
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloper = await _context.XDeveloper.FindAsync(id);
            if (xDeveloper == null)
            {
                return NotFound();
            }
            return View(xDeveloper);
        }

        #region 

        // POST: DeveloperZone/XDevelopers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,HomeCoverImgMD5,DOJoining")] XDeveloper xDeveloper)
        {
            if (id != xDeveloper.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xDeveloper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XDeveloperExists(xDeveloper.Id))
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
            return View(xDeveloper);
        }

        #region 

        // GET: DeveloperZone/XDevelopers/Delete/5
        #endregion
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloper = await _context.XDeveloper
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xDeveloper == null)
            {
                return NotFound();
            }

            return View(xDeveloper);
        }

        #region 

        // POST: DeveloperZone/XDevelopers/Delete/5
        #endregion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var xDeveloper = await _context.XDeveloper.FindAsync(id);
            _context.XDeveloper.Remove(xDeveloper);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool XDeveloperExists(Guid id)
        {
            return _context.XDeveloper.Any(e => e.Id == id);
        }
    }
}
