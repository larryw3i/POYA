using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.DeveloperZone.Models;
using POYA.Data;

namespace POYA.Areas.DeveloperZone.Controllers
{
    [Area("DeveloperZone")]
    public class XDevelopersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public XDevelopersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: DeveloperZone/XDevelopers
        public async Task<IActionResult> Index()
        {
            return View(await _context.XDeveloper.ToListAsync());
        }

        // GET: DeveloperZone/XDevelopers/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloper = await _context.XDeveloper
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xDeveloper == null)
            {
                return NotFound();
            }

            return View(xDeveloper);
        }

        // GET: DeveloperZone/XDevelopers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DeveloperZone/XDevelopers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,HomeCoverImgMD5,DOJoining")] XDeveloper xDeveloper)
        {
            if (ModelState.IsValid)
            {
                xDeveloper.Id = Guid.NewGuid();
                _context.Add(xDeveloper);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(xDeveloper);
        }

        // GET: DeveloperZone/XDevelopers/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloper = await _context.XDeveloper.FindAsync(id);
            if (xDeveloper == null)
            {
                return NotFound();
            }
            return View(xDeveloper);
        }

        // POST: DeveloperZone/XDevelopers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,HomeCoverImgMD5,DOJoining")] XDeveloper xDeveloper)
        {
            if (id != xDeveloper.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xDeveloper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XDeveloperExists(xDeveloper.Id))
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
            return View(xDeveloper);
        }

        // GET: DeveloperZone/XDevelopers/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xDeveloper = await _context.XDeveloper
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xDeveloper == null)
            {
                return NotFound();
            }

            return View(xDeveloper);
        }

        // POST: DeveloperZone/XDevelopers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var xDeveloper = await _context.XDeveloper.FindAsync(id);
            _context.XDeveloper.Remove(xDeveloper);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool XDeveloperExists(Guid id)
        {
            return _context.XDeveloper.Any(e => e.Id == id);
        }
    }
}
