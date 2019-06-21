using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using POYA.Areas.EduHub.Controllers;
using POYA.Areas.XSchool.Models;
using POYA.Areas.XUserFile.Controllers;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.XSchool.Controllers
{
    [Area("XSchool")]
    public class LSchoolAdminsController : Controller
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
        //  private readonly MimeHelper _mimeHelper;
        private readonly XUserFileHelper _xUserFileHelper;
        public LSchoolAdminsController(
            //  MimeHelper mimeHelper,
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
            //  _mimeHelper = mimeHelper;
            _xUserFileHelper = new XUserFileHelper();
        }
        #endregion


        #region 

        // GET: XSchool/LSchoolAdmins
        #endregion
        public async Task<IActionResult> Index()
        {
            return View(await _context.LSchoolAdmin.ToListAsync());
        }

        #region 

        // GET: XSchool/LSchoolAdmins/Details/5
        #endregion
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolAdmin = await _context.LSchoolAdmin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSchoolAdmin == null)
            {
                return NotFound();
            }

            return View(lSchoolAdmin);
        }

        #region 

        // GET: XSchool/LSchoolAdmins/Create
        #endregion
        public IActionResult Create()
        {
            return View();
        }

        #region 

        // POST: XSchool/LSchoolAdmins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> Create([Bind("Id,UserId,LSchoolId,DOConfirmation,IsConfirmed,IsCancelled")] LSchoolAdmin lSchoolAdmin)
        {
            if (ModelState.IsValid)
            {
                lSchoolAdmin.Id = Guid.NewGuid();
                _context.Add(lSchoolAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lSchoolAdmin);
        }

        #region 

        // GET: XSchool/LSchoolAdmins/Edit/5
        #endregion
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolAdmin = await _context.LSchoolAdmin.FindAsync(id);
            if (lSchoolAdmin == null)
            {
                return NotFound();
            }
            return View(lSchoolAdmin);
        }

        #region 

        // POST: XSchool/LSchoolAdmins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,LSchoolId,DOConfirmation,IsConfirmed,IsCancelled")] LSchoolAdmin lSchoolAdmin)
        {
            if (id != lSchoolAdmin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lSchoolAdmin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LSchoolAdminExists(lSchoolAdmin.Id))
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
            return View(lSchoolAdmin);
        }

        #region 

        // GET: XSchool/LSchoolAdmins/Delete/5
        #endregion
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolAdmin = await _context.LSchoolAdmin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSchoolAdmin == null)
            {
                return NotFound();
            }

            return View(lSchoolAdmin);
        }

        #region 

        // POST: XSchool/LSchoolAdmins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lSchoolAdmin = await _context.LSchoolAdmin.FindAsync(id);
            _context.LSchoolAdmin.Remove(lSchoolAdmin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LSchoolAdminExists(Guid id)
        {
            return _context.LSchoolAdmin.Any(e => e.Id == id);
        }

        #region DEPOLLUTION

        #endregion
    }
}
