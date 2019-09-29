using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.WeEduHub.Models;
using POYA.Data;

namespace POYA.Areas.WeEduHub.Controllers
{
    [Authorize(Roles="dd124f0f-1aa5-4aee-9297-f87a1e7a4183")]
    [Area("WeEduHub")]
    public class FunCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FunCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WeEduHub/FunComments
        public async Task<IActionResult> Index()
        {
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CommentUserId,WeArticleId,CommentContent,DOCommenting,IsShielded,DOShielding")] FunComment funComment)
        {
            if (ModelState.IsValid)
            {
                funComment.Id = Guid.NewGuid();
                _context.Add(funComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
