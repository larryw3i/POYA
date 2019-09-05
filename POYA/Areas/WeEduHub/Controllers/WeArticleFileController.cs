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
    public class WeArticleFileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WeArticleFileController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: WeEduHub/WeArticleFile
        public async Task<IActionResult> Index()
        {
            return View(await _context.WeArticleFile.ToListAsync());
        }

        // GET: WeEduHub/WeArticleFile/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticleFile = await _context.WeArticleFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weArticleFile == null)
            {
                return NotFound();
            }

            return View(weArticleFile);
        }

        // GET: WeEduHub/WeArticleFile/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: WeEduHub/WeArticleFile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,SHA256HexString,Name,DOUploading")] WeArticleFile weArticleFile)
        {
            if (ModelState.IsValid)
            {
                weArticleFile.Id = Guid.NewGuid();
                _context.Add(weArticleFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(weArticleFile);
        }

        // GET: WeEduHub/WeArticleFile/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticleFile = await _context.WeArticleFile.FindAsync(id);
            if (weArticleFile == null)
            {
                return NotFound();
            }
            return View(weArticleFile);
        }

        // POST: WeEduHub/WeArticleFile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,SHA256HexString,Name,DOUploading")] WeArticleFile weArticleFile)
        {
            if (id != weArticleFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(weArticleFile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WeArticleFileExists(weArticleFile.Id))
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
            return View(weArticleFile);
        }

        // GET: WeEduHub/WeArticleFile/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var weArticleFile = await _context.WeArticleFile
                .FirstOrDefaultAsync(m => m.Id == id);
            if (weArticleFile == null)
            {
                return NotFound();
            }

            return View(weArticleFile);
        }

        // POST: WeEduHub/WeArticleFile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var weArticleFile = await _context.WeArticleFile.FindAsync(id);
            _context.WeArticleFile.Remove(weArticleFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WeArticleFileExists(Guid id)
        {
            return _context.WeArticleFile.Any(e => e.Id == id);
        }
    }
}
