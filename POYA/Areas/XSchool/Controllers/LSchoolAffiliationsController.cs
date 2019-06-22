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
    public class LSchoolAffiliationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LSchoolAffiliationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: XSchool/LSchoolAffiliations
        public async Task<IActionResult> Index()
        {
            return View(await _context.LSchoolAffiliation.ToListAsync());
        }

        // GET: XSchool/LSchoolAffiliations/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolAffiliation = await _context.LSchoolAffiliation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSchoolAffiliation == null)
            {
                return NotFound();
            }

            return View(lSchoolAffiliation);
        }

        // GET: XSchool/LSchoolAffiliations/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: XSchool/LSchoolAffiliations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MasterLSchoolId,AffiliateLSchoolId,Commit")] LSchoolAffiliation lSchoolAffiliation)
        {
            if (ModelState.IsValid)
            {
                lSchoolAffiliation.Id = Guid.NewGuid();
                _context.Add(lSchoolAffiliation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lSchoolAffiliation);
        }

        // GET: XSchool/LSchoolAffiliations/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolAffiliation = await _context.LSchoolAffiliation.FindAsync(id);
            if (lSchoolAffiliation == null)
            {
                return NotFound();
            }
            return View(lSchoolAffiliation);
        }

        // POST: XSchool/LSchoolAffiliations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,MasterLSchoolId,AffiliateLSchoolId,Commit")] LSchoolAffiliation lSchoolAffiliation)
        {
            if (id != lSchoolAffiliation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lSchoolAffiliation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LSchoolAffiliationExists(lSchoolAffiliation.Id))
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
            return View(lSchoolAffiliation);
        }

        // GET: XSchool/LSchoolAffiliations/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolAffiliation = await _context.LSchoolAffiliation
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSchoolAffiliation == null)
            {
                return NotFound();
            }

            return View(lSchoolAffiliation);
        }

        // POST: XSchool/LSchoolAffiliations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lSchoolAffiliation = await _context.LSchoolAffiliation.FindAsync(id);
            _context.LSchoolAffiliation.Remove(lSchoolAffiliation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LSchoolAffiliationExists(Guid id)
        {
            return _context.LSchoolAffiliation.Any(e => e.Id == id);
        }
    }
}
