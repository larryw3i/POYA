using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.EduHub.Models;
using POYA.Data;

namespace POYA.Areas.EduHub.Controllers
{
    [Area("EduHub")]
    [Authorize]
    public class EArticleCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EArticleCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/EArticleComments
        public async Task<IActionResult> Index()
        {
            return View(await _context.EArticleComment.ToListAsync());
        }

        // GET: EduHub/EArticleComments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eArticleComment = await _context.EArticleComment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eArticleComment == null)
            {
                return NotFound();
            }

            return View(eArticleComment);
        }

        // GET: EduHub/EArticleComments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/EArticleComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EArticleId,Comment,DOComment,IsLegal")] EArticleComment eArticleComment)
        {
            if (ModelState.IsValid)
            {
                eArticleComment.Id = Guid.NewGuid();
                _context.Add(eArticleComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eArticleComment);
        }

        // GET: EduHub/EArticleComments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eArticleComment = await _context.EArticleComment.FindAsync(id);
            if (eArticleComment == null)
            {
                return NotFound();
            }
            return View(eArticleComment);
        }

        // POST: EduHub/EArticleComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,EArticleId,Comment,DOComment,IsLegal")] EArticleComment eArticleComment)
        {
            if (id != eArticleComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eArticleComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EArticleCommentExists(eArticleComment.Id))
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
            return View(eArticleComment);
        }

        // GET: EduHub/EArticleComments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eArticleComment = await _context.EArticleComment
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eArticleComment == null)
            {
                return NotFound();
            }

            return View(eArticleComment);
        }

        // POST: EduHub/EArticleComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eArticleComment = await _context.EArticleComment.FindAsync(id);
            _context.EArticleComment.Remove(eArticleComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EArticleCommentExists(Guid id)
        {
            return _context.EArticleComment.Any(e => e.Id == id);
        }
    }
}
