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
    public class LSchoolAdminsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LSchoolAdminsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: XSchool/LSchoolAdmins
        public async Task<IActionResult> Index()
        {
            return View(await _context.LSchoolAdmin.ToListAsync());
        }

        // GET: XSchool/LSchoolAdmins/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolAdmin = await _context.LSchoolAdmin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSchoolAdmin == null)
            {
                return NotFound();
            }

            return View(lSchoolAdmin);
        }

        // GET: XSchool/LSchoolAdmins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: XSchool/LSchoolAdmins/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,LSchoolId,DOConfirmation,IsConfirmed,IsCancelled")] LSchoolAdmin lSchoolAdmin)
        {
            if (ModelState.IsValid)
            {
                lSchoolAdmin.Id = Guid.NewGuid();
                _context.Add(lSchoolAdmin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(lSchoolAdmin);
        }

        // GET: XSchool/LSchoolAdmins/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolAdmin = await _context.LSchoolAdmin.FindAsync(id);
            if (lSchoolAdmin == null)
            {
                return NotFound();
            }
            return View(lSchoolAdmin);
        }

        // POST: XSchool/LSchoolAdmins/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,LSchoolId,DOConfirmation,IsConfirmed,IsCancelled")] LSchoolAdmin lSchoolAdmin)
        {
            if (id != lSchoolAdmin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lSchoolAdmin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LSchoolAdminExists(lSchoolAdmin.Id))
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
            return View(lSchoolAdmin);
        }

        // GET: XSchool/LSchoolAdmins/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lSchoolAdmin = await _context.LSchoolAdmin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lSchoolAdmin == null)
            {
                return NotFound();
            }

            return View(lSchoolAdmin);
        }

        // POST: XSchool/LSchoolAdmins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var lSchoolAdmin = await _context.LSchoolAdmin.FindAsync(id);
            _context.LSchoolAdmin.Remove(lSchoolAdmin);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LSchoolAdminExists(Guid id)
        {
            return _context.LSchoolAdmin.Any(e => e.Id == id);
        }
    }
}
