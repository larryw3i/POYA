using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.XSchool.Models;
using POYA.Data;

namespace POYA.Areas.XSchool.Controllers
{
    [Area("XSchool")]
    public class LSchoolNewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LSchoolNewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: XSchool/LSchoolNews
        public async Task<IActionResult> Index()
        {
            return View(await _context.LSchoolNews.ToListAsync());
        }

        // GET: XSchool/LSchoolNews/Details/5
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

        // GET: XSchool/LSchoolNews/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: XSchool/LSchoolNews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: XSchool/LSchoolNews/Edit/5
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

        // POST: XSchool/LSchoolNews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        // GET: XSchool/LSchoolNews/Delete/5
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

        // POST: XSchool/LSchoolNews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
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
