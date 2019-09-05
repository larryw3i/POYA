using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.iEduHub.Models;
using POYA.Data;

namespace POYA.Areas.WeEduHub.Controllers
{
    [Area("WeEduHub")]
    public class WeArticleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeArticleController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WeEduHub/WeArticle
        public async Task<IActionResult> Index()
        {
            return View(await _context.WeArticle.ToListAsync());
        }

        // GET: WeEduHub/WeArticle/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticle = await _context.WeArticle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weArticle == null)
            {
                return NotFound();
            }

            return View(weArticle);
        }

        // GET: WeEduHub/WeArticle/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WeEduHub/WeArticle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,SetId,Title,Content,DOPublishing,DOModifying")] WeArticle weArticle)
        {
            if (ModelState.IsValid)
            {
                weArticle.Id = Guid.NewGuid();
                _context.Add(weArticle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weArticle);
        }

        // GET: WeEduHub/WeArticle/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticle = await _context.WeArticle.FindAsync(id);
            if (weArticle == null)
            {
                return NotFound();
            }
            return View(weArticle);
        }

        // POST: WeEduHub/WeArticle/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,SetId,Title,Content,DOPublishing,DOModifying")] WeArticle weArticle)
        {
            if (id != weArticle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weArticle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeArticleExists(weArticle.Id))
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
            return View(weArticle);
        }

        // GET: WeEduHub/WeArticle/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticle = await _context.WeArticle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weArticle == null)
            {
                return NotFound();
            }

            return View(weArticle);
        }

        // POST: WeEduHub/WeArticle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var weArticle = await _context.WeArticle.FindAsync(id);
            _context.WeArticle.Remove(weArticle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeArticleExists(Guid id)
        {
            return _context.WeArticle.Any(e => e.Id == id);
        }
    }
}
