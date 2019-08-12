using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.LAdmin.Models;
using POYA.Data;

namespace POYA.Areas.LAdmin.Controllers
{
    [Area("LAdmin")]
    [Authorize]
    public class LAuditsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LAuditsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LAudits
        public async Task<IActionResult> Index()
        {
            return View(await _context.LAudit.ToListAsync());
        }

        // GET: LAudits/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAudit = await _context.LAudit
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lAudit == null)
            {
                return NotFound();
            }

            return View(lAudit);
        }

        // GET: LAudits/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LAudits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContentId,DOAudit,InspectorId,AuditResultId")] LAudit lAudit)
        {
            if (ModelState.IsValid)
            {
                lAudit.Id = Guid.NewGuid();
                _context.Add(lAudit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lAudit);
        }

        // GET: LAudits/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAudit = await _context.LAudit.FindAsync(id);
            if (lAudit == null)
            {
                return NotFound();
            }
            return View(lAudit);
        }

        // POST: LAudits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ContentId,DOAudit,InspectorId,AuditResultId")] LAudit lAudit)
        {
            if (id != lAudit.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lAudit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LAuditExists(lAudit.Id))
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
            return View(lAudit);
        }

        // GET: LAudits/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lAudit = await _context.LAudit
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lAudit == null)
            {
                return NotFound();
            }

            return View(lAudit);
        }

        // POST: LAudits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lAudit = await _context.LAudit.FindAsync(id);
            _context.LAudit.Remove(lAudit);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LAuditExists(Guid id)
        {
            return _context.LAudit.Any(e => e.Id == id);
        }
    }
}
