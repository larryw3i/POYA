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
using POYA.Areas.FunFiles.Controllers;
using POYA.Areas.WeEduHub.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.WeEduHub.Controllers
{
    [Authorize]
    [Area("WeEduHub")]
    public class WeArticleFileController : Controller
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
        private readonly FunFilesHelper _funFilesHelper;
        public WeArticleFileController(
            FunFilesHelper funFilesHelper,
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
            _funFilesHelper=funFilesHelper;
        }
        #endregion

        // GET: WeEduHub/WeArticleFile
        public async Task<IActionResult> Index()
        {
            return View(await _context.WeArticleFile.ToListAsync());
        }

        // GET: WeEduHub/WeArticleFile/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticleFile = await _context.WeArticleFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weArticleFile == null)
            {
                return NotFound();
            }

            return View(weArticleFile);
        }

        // GET: WeEduHub/WeArticleFile/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WeEduHub/WeArticleFile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,SHA256HexString,Name,DOUploading")] WeArticleFile weArticleFile)
        {
            if (ModelState.IsValid)
            {
                weArticleFile.Id = Guid.NewGuid();
                _context.Add(weArticleFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weArticleFile);
        }

        // GET: WeEduHub/WeArticleFile/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticleFile = await _context.WeArticleFile.FindAsync(id);
            if (weArticleFile == null)
            {
                return NotFound();
            }
            return View(weArticleFile);
        }

        // POST: WeEduHub/WeArticleFile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,SHA256HexString,Name,DOUploading")] WeArticleFile weArticleFile)
        {
            if (id != weArticleFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weArticleFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeArticleFileExists(weArticleFile.Id))
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
            return View(weArticleFile);
        }

        // GET: WeEduHub/WeArticleFile/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticleFile = await _context.WeArticleFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weArticleFile == null)
            {
                return NotFound();
            }

            return View(weArticleFile);
        }

        // POST: WeEduHub/WeArticleFile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var weArticleFile = await _context.WeArticleFile.FindAsync(id);
            _context.WeArticleFile.Remove(weArticleFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeArticleFileExists(Guid id)
        {
            return _context.WeArticleFile.Any(e => e.Id == id);
        }
    }
}
