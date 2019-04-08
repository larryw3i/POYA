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
using POYA.Data;
using POYA.Models;
using POYA.Unities.Helpers;

namespace POYA.Controllers
{
    public class X_doveUserFileTagController : Controller
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
        public X_doveUserFileTagController(
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

        // GET: X_doveUserFileTag
        public async Task<IActionResult> Index()
        {
            return View(await _context.X_doveUserFileTag.ToListAsync());
        }

        // GET: X_doveUserFileTag/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var x_doveUserFileTag = await _context.X_doveUserFileTag
                .FirstOrDefaultAsync(m => m.Id == id);
            if (x_doveUserFileTag == null)
            {
                return NotFound();
            }

            return View(x_doveUserFileTag);
        }

        // GET: X_doveUserFileTag/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: X_doveUserFileTag/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,FileId,Name,DOGeneration,InTagId")] X_doveUserFileTag x_doveUserFileTag)
        {
            if (ModelState.IsValid)
            {
                x_doveUserFileTag.Id = Guid.NewGuid();
                _context.Add(x_doveUserFileTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(x_doveUserFileTag);
        }

        // GET: X_doveUserFileTag/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var x_doveUserFileTag = await _context.X_doveUserFileTag.FindAsync(id);
            if (x_doveUserFileTag == null)
            {
                return NotFound();
            }
            return View(x_doveUserFileTag);
        }

        // POST: X_doveUserFileTag/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,FileId,Name,DOGeneration,InTagId")] X_doveUserFileTag x_doveUserFileTag)
        {
            if (id != x_doveUserFileTag.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(x_doveUserFileTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!X_doveUserFileTagExists(x_doveUserFileTag.Id))
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
            return View(x_doveUserFileTag);
        }

        // GET: X_doveUserFileTag/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var x_doveUserFileTag = await _context.X_doveUserFileTag
                .FirstOrDefaultAsync(m => m.Id == id);
            if (x_doveUserFileTag == null)
            {
                return NotFound();
            }

            return View(x_doveUserFileTag);
        }

        // POST: X_doveUserFileTag/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var x_doveUserFileTag = await _context.X_doveUserFileTag.FindAsync(id);
            _context.X_doveUserFileTag.Remove(x_doveUserFileTag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool X_doveUserFileTagExists(Guid id)
        {
            return _context.X_doveUserFileTag.Any(e => e.Id == id);
        }
    }
}
