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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using POYA.Areas.FunAdmin.Controllers;
using POYA.Areas.FunFiles.Controllers;
using POYA.Areas.WeEduHub.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.WeEduHub.Controllers
{
    [Area("WeEduHub")]
    public class FunCommentsController : Controller
    {
        #region     DI
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly WeEduHubHelper _weEduHubHelper;
        private readonly FunFilesHelper _funFilesHelper;
        private readonly WeEduHubArticleClassHelper _weEduHubArticleClassHelper;
        private readonly FunAdminHelper _funAdminHelper;
        public FunCommentsController(
            HtmlSanitizer htmlSanitizer,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnv,
            IStringLocalizer<Program> localizer)
        {
            _htmlSanitizer = htmlSanitizer;
            _webHostEnv = webHostEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
            _weEduHubArticleClassHelper=new WeEduHubArticleClassHelper(_webHostEnv );
            _weEduHubHelper=new WeEduHubHelper();
            _funFilesHelper=new FunFilesHelper();
            _funAdminHelper=new FunAdminHelper(_localizer,_context);
        }
        #endregion

        // GET: WeEduHub/FunComments
        public async Task<IActionResult> Index(Guid WeArticleId)
        {
            var _FunComments=await _context.FunComment.Where(p=>p.WeArticleId==WeArticleId).ToListAsync();
            
            return View(await _context.FunComment.ToListAsync());
        }

        // GET: WeEduHub/FunComments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funComment = await _context.FunComment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funComment == null)
            {
                return NotFound();
            }

            return View(funComment);
        }

        // GET: WeEduHub/FunComments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WeEduHub/FunComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("WeArticleId,CommentContent")] FunComment funComment)
        {
            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var _funComment=new FunComment()
                {
                    Id=Guid.NewGuid(),
                    CommentContent=funComment.CommentContent,
                    CommentUserId=_UserId,
                    DOCommenting=DateTimeOffset.Now,
                    IsShielded=false,
                    WeArticleId=funComment.WeArticleId,
                };
                
                await _context.AddAsync(_funComment);
                await _context.SaveChangesAsync();
                return Ok();    //   RedirectToAction(nameof(Index));
            }
            return View(funComment);
        }

        // GET: WeEduHub/FunComments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funComment = await _context.FunComment.FindAsync(id);
            if (funComment == null)
            {
                return NotFound();
            }
            return View(funComment);
        }

        // POST: WeEduHub/FunComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CommentUserId,WeArticleId,CommentContent,DOCommenting,IsShielded,DOShielding")] FunComment funComment)
        {
            if (id != funComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunCommentExists(funComment.Id))
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
            return View(funComment);
        }

        // GET: WeEduHub/FunComments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funComment = await _context.FunComment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funComment == null)
            {
                return NotFound();
            }

            return View(funComment);
        }

        // POST: WeEduHub/FunComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var funComment = await _context.FunComment.FindAsync(id);
            _context.FunComment.Remove(funComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FunCommentExists(Guid id)
        {
            return _context.FunComment.Any(e => e.Id == id);
        }
    }
}
