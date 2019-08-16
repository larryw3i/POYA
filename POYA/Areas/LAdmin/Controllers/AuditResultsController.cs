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
using POYA.Areas.LAdmin.Models;
using POYA.Data;
using POYA.Unities.Helpers;
using POYA.Areas.EduHub.Controllers;

namespace POYA.Areas.LAdmin.Controllers
{
    [Area("LAdmin")]
    [Authorize(Roles="ADMINISTRATOR")]
    public class AuditResultsController : Controller
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
        private readonly LAdminHelper _lAdminHelper;
        public AuditResultsController(
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
            _lAdminHelper=new  LAdminHelper(_localizer,_context);
        }

        #endregion

        // GET: LAdmin/AuditResults
        public async Task<IActionResult> Index()
        {
            return View(await _context.AuditResult.ToListAsync());
        }

        // GET: LAdmin/AuditResults/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditResult = await _context.AuditResult
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auditResult == null)
            {
                return NotFound();
            }

            return View(auditResult);
        }

        // GET: LAdmin/AuditResults/Create
        public async Task<IActionResult> Create(Guid? ComplainantId)
        {
            if (ComplainantId == null)
            {
                return NotFound();
            }

            var _ContentId=await _context.UserComplaint.Where(p=>p.Id==ComplainantId).Select(p=>p.ContentId).FirstOrDefaultAsync();

            var _AuditResult = new AuditResult()
            {
                ComplainantId= ComplainantId ?? Guid.Empty,
                ContentId=_ContentId,
                ContentTitle=await _context.EArticle.Where(p=>p.Id==_ContentId).Select(p=>p.Title).FirstOrDefaultAsync(),
                IllegalityTypeSelectListItems = _lAdminHelper.GetIllegalityTypeSelectListItems()
            };

            return View(_AuditResult);
        }

        // POST: LAdmin/AuditResults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContentId,IsContentLegal,IllegalityType,AuditComment")] AuditResult auditResult)
        {
            if (ModelState.IsValid)
            {
                if(!await _context.UserComplaint
                    .Select(p=>p.Id)
                    .Union(_context.LAudit
                    .Select(p=>p.Id))
                    .ContainsAsync(auditResult.ComplainantId))
                        return NotFound();
                        
                auditResult.Id = Guid.NewGuid();
                _context.Add(auditResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(auditResult);
        }

        // GET: LAdmin/AuditResults/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditResult = await _context.AuditResult.FindAsync(id);
            if (auditResult == null)
            {
                return NotFound();
            }
            return View(auditResult);
        }

        // POST: LAdmin/AuditResults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ContentId,IsContentLegal,IllegalityType,AuditComment")] AuditResult auditResult)
        {
            if (id != auditResult.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auditResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuditResultExists(auditResult.Id))
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
            return View(auditResult);
        }

        // GET: LAdmin/AuditResults/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditResult = await _context.AuditResult
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auditResult == null)
            {
                return NotFound();
            }

            return View(auditResult);
        }

        // POST: LAdmin/AuditResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var auditResult = await _context.AuditResult.FindAsync(id);
            _context.AuditResult.Remove(auditResult);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuditResultExists(Guid id)
        {
            return _context.AuditResult.Any(e => e.Id == id);
        }

        public async Task<ActionResult> GetContentDetails(Guid ContentId){
            if(await _context.EArticle.AnyAsync(p=>p.Id==ContentId)){
                return Content(await _context.EArticle.Where(p=>p.Id==ContentId).Select(p=>p.Content).FirstOrDefaultAsync());
            }
            return NotFound();
        }
    }
}
