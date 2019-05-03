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
    public class EArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/EArticles
        public async Task<IActionResult> Index()
        {
            return View(await _context.EArticle.ToListAsync());
        }

        // GET: EduHub/EArticles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eArticle = await _context.EArticle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eArticle == null)
            {
                return NotFound();
            }

            return View(eArticle);
        }

        // GET: EduHub/EArticles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/EArticles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,SubjectId,GradeId,TypeId,VideoId,Title,Content,ContentType,IsLegal")] EArticle eArticle)
        {
            if (ModelState.IsValid)
            {
                eArticle.Id = Guid.NewGuid();
                _context.Add(eArticle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eArticle);
        }

        // GET: EduHub/EArticles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eArticle = await _context.EArticle.FindAsync(id);
            if (eArticle == null)
            {
                return NotFound();
            }
            return View(eArticle);
        }

        // POST: EduHub/EArticles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,SubjectId,GradeId,TypeId,VideoId,Title,Content,ContentType,IsLegal")] EArticle eArticle)
        {
            if (id != eArticle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eArticle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EArticleExists(eArticle.Id))
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
            return View(eArticle);
        }

        // GET: EduHub/EArticles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eArticle = await _context.EArticle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eArticle == null)
            {
                return NotFound();
            }

            return View(eArticle);
        }

        // POST: EduHub/EArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eArticle = await _context.EArticle.FindAsync(id);
            _context.EArticle.Remove(eArticle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EArticleExists(Guid id)
        {
            return _context.EArticle.Any(e => e.Id == id);
        }
    }
}
