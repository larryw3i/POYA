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

namespace POYA.Areas.LAdmin.Controllers
{
    [Area("LAdmin")]
    [Authorize]
    public class UserComplaintsController : Controller
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
        public UserComplaintsController(
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

        // GET: UserComplaints
        public async Task<IActionResult> Index()
        {
            var _UserComplaint=await _context.UserComplaint.ToListAsync();
            
            var User_ =await _userManager.GetUserAsync(User);

            var IsChecking=await _userManager.IsInRoleAsync(User_, X_DOVEValues._administrator);


            _UserComplaint.ForEach( p =>{
                p.ComplainantName= _userManager.FindByIdAsync(p.ComplainantId).GetAwaiter().GetResult()?.UserName??_localizer["Logged off user"];
                p.ReceptionistName= _userManager.FindByIdAsync(p.ReceptionistId).GetAwaiter().GetResult()?.UserName??_localizer["pending items"];
                p.ContentTitle= _lAdminHelper.GetContentTitleAsync(p.ContentId).GetAwaiter().GetResult();
                p.IllegalityTypeString=_lAdminHelper.GetIllegalityTypeSelectListItems().Where(i=>i.Value==p.IllegalityType).Select(o=>o.Text).FirstOrDefault();
            });


            ViewData[nameof(IsChecking)]=IsChecking;
            ViewData["UserId"]=User_.Id;

            return View(_UserComplaint);
        }

        // GET: UserComplaints/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userComplaint = await _context.UserComplaint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userComplaint == null)
            {
                return NotFound();
            }

            return View(userComplaint);
        }

        // GET: UserComplaints/Create
        public IActionResult Create(Guid _ContentId)
        {
            var _UserComplaint=new UserComplaint{
                ContentId=_ContentId,
                IllegalityTypeSelectListItems=_lAdminHelper.GetIllegalityTypeSelectListItems(),
                ContentTitle=_lAdminHelper.GetContentTitleAsync(_ContentId).GetAwaiter().GetResult()
            };
            return View(_UserComplaint);
        }

        // POST: UserComplaints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContentId,Description,IllegalityType")] UserComplaint userComplaint)
        {
            if (ModelState.IsValid)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

                userComplaint.Id = Guid.NewGuid();
                userComplaint.ComplainantId=UserId_;
                userComplaint.DOComplaint=DateTimeOffset.Now;

                if(!_lAdminHelper.GetIllegalityTypeSelectListItems()
                    .Select(p=>p.Value)
                    .Contains(userComplaint.IllegalityType))
                    userComplaint.IllegalityType="110";

                _context.Add(userComplaint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userComplaint);
        }

        // GET: UserComplaints/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var User_ =await _userManager.GetUserAsync(User);

            var IsChecking=await _userManager.IsInRoleAsync(User_, X_DOVEValues._administrator);

            var userComplaint = await _context.UserComplaint.FindAsync(id);
            if (userComplaint == null)
            {
                return NotFound();
            }

            userComplaint.ComplainantName= _userManager.FindByIdAsync(userComplaint.ComplainantId).GetAwaiter().GetResult().UserName;
            userComplaint.ReceptionistName= _userManager.FindByIdAsync(userComplaint.ReceptionistId).GetAwaiter().GetResult()?.UserName??_localizer["pending items"];
            userComplaint.ContentTitle= _lAdminHelper.GetContentTitleAsync(userComplaint.ContentId).GetAwaiter().GetResult();
            userComplaint.IllegalityTypeSelectListItems=_lAdminHelper.GetIllegalityTypeSelectListItems();

            ViewData[nameof(IsChecking)]=IsChecking;
            ViewData["UserId"]=User_.Id;

            return View(userComplaint);
        }

        // POST: UserComplaints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,IllegalityType")] UserComplaint userComplaint)
        {
            if (id != userComplaint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

                    var _UserComplaint=await _context.UserComplaint.FirstOrDefaultAsync(p=>p.Id==id && p.ComplainantId==UserId_);

                    if(_UserComplaint==null){
                        return NotFound();
                    }

                    _UserComplaint.Description=userComplaint.Description;
                    _UserComplaint.DOModifying=DateTimeOffset.Now;

                    if(_lAdminHelper.GetIllegalityTypeSelectListItems()
                        .Select(p=>p.Value)
                        .Contains(userComplaint.IllegalityType))
                            _UserComplaint.IllegalityType=userComplaint.IllegalityType;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserComplaintExists(userComplaint.Id))
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
            return View(userComplaint);
        }

        // GET: UserComplaints/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            
            var userComplaint = await _context.UserComplaint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userComplaint == null)
            {
                return NotFound();
            }

            return View(userComplaint);
        }

        // POST: UserComplaints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles="ADMINISTRATOR")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userComplaint = await _context.UserComplaint.FindAsync(id);
            _context.UserComplaint.Remove(userComplaint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        private bool UserComplaintExists(Guid id)
        {
            return _context.UserComplaint.Any(e => e.Id == id);
        }

    }
}
