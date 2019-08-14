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
    public class AuditResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LAdmin/AuditResults
        public async Task<IActionResult> Index()
        {
            return View(await _context.AuditResult.ToListAsync());
        }

        // GET: LAdmin/AuditResults/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditResult = await _context.AuditResult
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auditResult == null)
            {
                return NotFound();
            }

            return View(auditResult);
        }

        // GET: LAdmin/AuditResults/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LAdmin/AuditResults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ContentId,IsContentLegal,IllegalityType,AuditComment")] AuditResult auditResult)
        {
            if (ModelState.IsValid)
            {
                auditResult.Id = Guid.NewGuid();
                _context.Add(auditResult);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(auditResult);
        }

        // GET: LAdmin/AuditResults/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditResult = await _context.AuditResult.FindAsync(id);
            if (auditResult == null)
            {
                return NotFound();
            }
            return View(auditResult);
        }

        // POST: LAdmin/AuditResults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ContentId,IsContentLegal,IllegalityType,AuditComment")] AuditResult auditResult)
        {
            if (id != auditResult.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auditResult);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuditResultExists(auditResult.Id))
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
            return View(auditResult);
        }

        // GET: LAdmin/AuditResults/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var auditResult = await _context.AuditResult
                .FirstOrDefaultAsync(m => m.Id == id);
            if (auditResult == null)
            {
                return NotFound();
            }

            return View(auditResult);
        }

        // POST: LAdmin/AuditResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var auditResult = await _context.AuditResult.FindAsync(id);
            _context.AuditResult.Remove(auditResult);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AuditResultExists(Guid id)
        {
            return _context.AuditResult.Any(e => e.Id == id);
        }
    }
}
