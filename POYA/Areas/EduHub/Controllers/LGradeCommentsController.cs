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
    public class LGradeCommentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LGradeCommentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/LGradeComments
        public async Task<IActionResult> Index()
        {
            return View(await _context.LGradeComments.ToListAsync());
        }

        // GET: EduHub/LGradeComments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lGradeComment = await _context.LGradeComments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lGradeComment == null)
            {
                return NotFound();
            }

            return View(lGradeComment);
        }

        // GET: EduHub/LGradeComments/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/LGradeComments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LGradeId,Comment")] LGradeComment lGradeComment)
        {
            if (ModelState.IsValid)
            {
                lGradeComment.Id = Guid.NewGuid();
                _context.Add(lGradeComment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lGradeComment);
        }

        // GET: EduHub/LGradeComments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lGradeComment = await _context.LGradeComments.FindAsync(id);
            if (lGradeComment == null)
            {
                return NotFound();
            }
            return View(lGradeComment);
        }

        // POST: EduHub/LGradeComments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,LGradeId,Comment")] LGradeComment lGradeComment)
        {
            if (id != lGradeComment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lGradeComment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LGradeCommentExists(lGradeComment.Id))
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
            return View(lGradeComment);
        }

        // GET: EduHub/LGradeComments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lGradeComment = await _context.LGradeComments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lGradeComment == null)
            {
                return NotFound();
            }

            return View(lGradeComment);
        }

        // POST: EduHub/LGradeComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lGradeComment = await _context.LGradeComments.FindAsync(id);
            _context.LGradeComments.Remove(lGradeComment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LGradeCommentExists(Guid id)
        {
            return _context.LGradeComments.Any(e => e.Id == id);
        }
    }
}
