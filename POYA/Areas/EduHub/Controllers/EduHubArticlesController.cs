using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.EduHub.Models;
using POYA.Data;

namespace POYA.Areas.EduHub.Controllers
{
    [Area("EduHub")]
    public class EduHubArticlesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EduHubArticlesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/EduHubArticles
        public async Task<IActionResult> Index()
        {
            return View(await _context.EduHubArticle.ToListAsync());
        }

        // GET: EduHub/EduHubArticles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubArticle = await _context.EduHubArticle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eduHubArticle == null)
            {
                return NotFound();
            }

            return View(eduHubArticle);
        }

        // GET: EduHub/EduHubArticles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/EduHubArticles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,SubjectId,GradeId,TypeId,VideoId,Title,Content,ContentType,IsLegal")] EduHubArticle eduHubArticle)
        {
            if (ModelState.IsValid)
            {
                eduHubArticle.Id = Guid.NewGuid();
                _context.Add(eduHubArticle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eduHubArticle);
        }

        // GET: EduHub/EduHubArticles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubArticle = await _context.EduHubArticle.FindAsync(id);
            if (eduHubArticle == null)
            {
                return NotFound();
            }
            return View(eduHubArticle);
        }

        // POST: EduHub/EduHubArticles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,SubjectId,GradeId,TypeId,VideoId,Title,Content,ContentType,IsLegal")] EduHubArticle eduHubArticle)
        {
            if (id != eduHubArticle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eduHubArticle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EduHubArticleExists(eduHubArticle.Id))
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
            return View(eduHubArticle);
        }

        // GET: EduHub/EduHubArticles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubArticle = await _context.EduHubArticle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eduHubArticle == null)
            {
                return NotFound();
            }

            return View(eduHubArticle);
        }

        // POST: EduHub/EduHubArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eduHubArticle = await _context.EduHubArticle.FindAsync(id);
            _context.EduHubArticle.Remove(eduHubArticle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EduHubArticleExists(Guid id)
        {
            return _context.EduHubArticle.Any(e => e.Id == id);
        }
    }
}
