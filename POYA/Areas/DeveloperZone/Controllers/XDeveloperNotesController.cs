using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Roles = "2E3DA548-49C7-4074-8A75-40730E503342")]
    public class XDeveloperNotesController : Controller
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
        private readonly ILogger<Program> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly MimeHelper _mimeHelper;
        private readonly XUserFileHelper _xUserFileHelper;
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly IConfiguration _configuration;
        public XDeveloperNotesController(
            IConfiguration configuration,
            HtmlSanitizer htmlSanitizer,
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
            _htmlSanitizer = htmlSanitizer;
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
        // GET: DeveloperZone/XDeveloperNotes

        #endregion
        public async Task<IActionResult> Index()
        {
            return View(await _context.XDeveloperNote.ToListAsync());
        }

        #region 

        // GET: DeveloperZone/XDeveloperNotes/Details/5
        #endregion
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloperNote = await _context.XDeveloperNote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xDeveloperNote == null)
            {
                return NotFound();
            }

            return View(xDeveloperNote);
        }

        #region 

        // GET: DeveloperZone/XDeveloperNotes/Create
        #endregion
        public IActionResult Create()
        {
            return View();
        }

        #region 

        // POST: DeveloperZone/XDeveloperNotes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,XDeveloperId,Title,Content,DOPublishing")] XDeveloperNote xDeveloperNote)
        {
            if (ModelState.IsValid)
            {
                xDeveloperNote.Id = Guid.NewGuid();
                _context.Add(xDeveloperNote);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(xDeveloperNote);
        }

        #region 

        // GET: DeveloperZone/XDeveloperNotes/Edit/5
        #endregion
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloperNote = await _context.XDeveloperNote.FindAsync(id);
            if (xDeveloperNote == null)
            {
                return NotFound();
            }
            return View(xDeveloperNote);
        }

        #region 

        // POST: DeveloperZone/XDeveloperNotes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,XDeveloperId,Title,Content,DOPublishing")] XDeveloperNote xDeveloperNote)
        {
            if (id != xDeveloperNote.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xDeveloperNote);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XDeveloperNoteExists(xDeveloperNote.Id))
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
            return View(xDeveloperNote);
        }

        #region 

        // GET: DeveloperZone/XDeveloperNotes/Delete/5
        #endregion
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloperNote = await _context.XDeveloperNote
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xDeveloperNote == null)
            {
                return NotFound();
            }

            return View(xDeveloperNote);
        }

        #region 

        // POST: DeveloperZone/XDeveloperNotes/Delete/5
        #endregion
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var xDeveloperNote = await _context.XDeveloperNote.FindAsync(id);
            _context.XDeveloperNote.Remove(xDeveloperNote);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool XDeveloperNoteExists(Guid id)
        {
            return _context.XDeveloperNote.Any(e => e.Id == id);
        }
    }
}
