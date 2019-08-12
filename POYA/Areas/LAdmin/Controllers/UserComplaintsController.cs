using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.LAdmin.Models;
using POYA.Data;

namespace POYA.Areas.LAdmin.Controllers
{
    public class UserComplaintsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserComplaintsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserComplaints
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserComplaint.ToListAsync());
        }

        // GET: UserComplaints/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userComplaint = await _context.UserComplaint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userComplaint == null)
            {
                return NotFound();
            }

            return View(userComplaint);
        }

        // GET: UserComplaints/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserComplaints/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,ContentId,DOComplaint,ReceptionistId,AuditResultId,Description,IllegalityType")] UserComplaint userComplaint)
        {
            if (ModelState.IsValid)
            {
                userComplaint.Id = Guid.NewGuid();
                _context.Add(userComplaint);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userComplaint);
        }

        // GET: UserComplaints/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userComplaint = await _context.UserComplaint.FindAsync(id);
            if (userComplaint == null)
            {
                return NotFound();
            }
            return View(userComplaint);
        }

        // POST: UserComplaints/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,ContentId,DOComplaint,ReceptionistId,AuditResultId,Description,IllegalityType")] UserComplaint userComplaint)
        {
            if (id != userComplaint.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userComplaint);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserComplaintExists(userComplaint.Id))
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
            return View(userComplaint);
        }

        // GET: UserComplaints/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userComplaint = await _context.UserComplaint
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userComplaint == null)
            {
                return NotFound();
            }

            return View(userComplaint);
        }

        // POST: UserComplaints/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var userComplaint = await _context.UserComplaint.FindAsync(id);
            _context.UserComplaint.Remove(userComplaint);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserComplaintExists(Guid id)
        {
            return _context.UserComplaint.Any(e => e.Id == id);
        }
    }
}
