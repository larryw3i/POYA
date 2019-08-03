using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using POYA.Areas.XAd.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.XAd.Controllers
{
    [Area("XAd")]
    [Authorize]
    public class LAdController : Controller
    {
        #region     DI
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly HtmlSanitizer _htmlSanitizer;
      
        public LAdController(
            HtmlSanitizer htmlSanitizer,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IHostingEnvironment hostingEnv,
            IStringLocalizer<Program> localizer)
        {
            _htmlSanitizer = htmlSanitizer;
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

        // GET: XAd/LAd
        public async Task<IActionResult> Index()
        {
            return View(await _context.LAds.ToListAsync());
        }

        // GET: XAd/LAd/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAd = await _context.LAds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lAd == null)
            {
                return NotFound();
            }

            return View(lAd);
        }

        // GET: XAd/LAd/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: XAd/LAd/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AdvertiserUserId,Title,Content,DOPublishing")] LAd lAd)
        {
            if (ModelState.IsValid)
            {
                lAd.Id = Guid.NewGuid();
                _context.Add(lAd);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lAd);
        }

        // GET: XAd/LAd/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAd = await _context.LAds.FindAsync(id);
            if (lAd == null)
            {
                return NotFound();
            }
            return View(lAd);
        }

        // POST: XAd/LAd/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,AdvertiserUserId,Title,Content,DOPublishing")] LAd lAd)
        {
            if (id != lAd.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lAd);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LAdExists(lAd.Id))
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
            return View(lAd);
        }

        // GET: XAd/LAd/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAd = await _context.LAds
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lAd == null)
            {
                return NotFound();
            }

            return View(lAd);
        }

        // POST: XAd/LAd/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lAd = await _context.LAds.FindAsync(id);
            _context.LAds.Remove(lAd);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LAdExists(Guid id)
        {
            return _context.LAds.Any(e => e.Id == id);
        }

        #region DEPOLLUTION
        public IActionResult UploadLAdImage(){
            return NoContent();
        }
        #endregion
    }
}
