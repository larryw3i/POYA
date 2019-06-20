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
using POYA.Areas.XSchool.Models;
using POYA.Areas.XUserFile.Controllers;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.XSchool.Controllers
{
    [Area("XSchool")]
    public class LSchoolNewsController : Controller
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
        private readonly MimeHelper _mimeHelper;
        private readonly XUserFileHelper _xUserFileHelper;
        public LSchoolNewsController(
            MimeHelper mimeHelper,
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
            _mimeHelper = mimeHelper;
            _xUserFileHelper = new XUserFileHelper();
        }
        #endregion

        #region 

        // GET: XSchool/LSchoolNews
        #endregion
        public async Task<IActionResult> Index()
        {
            return View(await _context.LSchoolNews.ToListAsync());
        }

        #region 

        // GET: XSchool/LSchoolNews/Details/5
        #endregion
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolNews = await _context.LSchoolNews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSchoolNews == null)
            {
                return NotFound();
            }

            return View(lSchoolNews);
        }

        #region 

        // GET: XSchool/LSchoolNews/Create
        #endregion
        public IActionResult Create()
        {
            return View();
        }

        #region 

        // POST: XSchool/LSchoolNews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> Create([Bind("Id,Title,Content,LSchoolId,PublisherId,DOPublishing,DOModifying")] LSchoolNews lSchoolNews)
        {
            if (ModelState.IsValid)
            {
                lSchoolNews.Id = Guid.NewGuid();
                _context.Add(lSchoolNews);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lSchoolNews);
        }

        #region 

        // GET: XSchool/LSchoolNews/Edit/5
        #endregion
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolNews = await _context.LSchoolNews.FindAsync(id);
            if (lSchoolNews == null)
            {
                return NotFound();
            }
            return View(lSchoolNews);
        }

        #region 

        // POST: XSchool/LSchoolNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Title,Content,LSchoolId,PublisherId,DOPublishing,DOModifying")] LSchoolNews lSchoolNews)
        {
            if (id != lSchoolNews.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lSchoolNews);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LSchoolNewsExists(lSchoolNews.Id))
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
            return View(lSchoolNews);
        }

        #region 

        // GET: XSchool/LSchoolNews/Delete/5
        #endregion
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolNews = await _context.LSchoolNews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSchoolNews == null)
            {
                return NotFound();
            }

            return View(lSchoolNews);
        }

        #region 

        // POST: XSchool/LSchoolNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lSchoolNews = await _context.LSchoolNews.FindAsync(id);
            _context.LSchoolNews.Remove(lSchoolNews);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LSchoolNewsExists(Guid id)
        {
            return _context.LSchoolNews.Any(e => e.Id == id);
        }
    }
}
