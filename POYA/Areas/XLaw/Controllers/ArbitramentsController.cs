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
using Microsoft.Extensions.Logging;
using POYA.Areas.XLaw.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.XLaw.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("XLaw")]
    public class ArbitramentsController : Controller
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
        private readonly ILogger<ArbitramentsController> _logger;
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly MimeHelper _mimeHelper;
        public ArbitramentsController(
            MimeHelper mimeHelper,
            HtmlSanitizer htmlSanitizer,
            ILogger<ArbitramentsController> logger,
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
            _mimeHelper = mimeHelper;
        }
        #endregion

        // GET: XLaw/Arbitraments
        public async Task<IActionResult> Index()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            return View(await _context.Arbitrament.ToListAsync());
        }

        // GET: XLaw/Arbitraments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arbitrament = await _context.Arbitrament
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arbitrament == null)
            {
                return NotFound();
            }

            return View(arbitrament);
        }

        // GET: XLaw/Arbitraments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: XLaw/Arbitraments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ComplaintId,AdminId,DOArbitrament,AdminComment,IsComplaintContentLegal")] Arbitrament arbitrament)
        {
            if (ModelState.IsValid)
            {
                arbitrament.Id = Guid.NewGuid();
                _context.Add(arbitrament);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(arbitrament);
        }

        // GET: XLaw/Arbitraments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arbitrament = await _context.Arbitrament.FindAsync(id);
            if (arbitrament == null)
            {
                return NotFound();
            }
            return View(arbitrament);
        }

        // POST: XLaw/Arbitraments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ComplaintId,AdminId,DOArbitrament,AdminComment,IsComplaintContentLegal")] Arbitrament arbitrament)
        {
            if (id != arbitrament.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(arbitrament);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArbitramentExists(arbitrament.Id))
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
            return View(arbitrament);
        }

        // GET: XLaw/Arbitraments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var arbitrament = await _context.Arbitrament
                .FirstOrDefaultAsync(m => m.Id == id);
            if (arbitrament == null)
            {
                return NotFound();
            }

            return View(arbitrament);
        }

        // POST: XLaw/Arbitraments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var arbitrament = await _context.Arbitrament.FindAsync(id);
            _context.Arbitrament.Remove(arbitrament);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArbitramentExists(Guid id)
        {
            return _context.Arbitrament.Any(e => e.Id == id);
        }
    }
}
