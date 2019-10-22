using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.LarryUserManagement.Models;
using POYA.Data;

namespace POYA.Areas.LarryUserManagement.Controllers
{
    [Area("LarryUserManagement")]
    public class LarryUsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LarryUsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LarryUserManagement/LarryUser
        public async Task<IActionResult> Index()
        {
            return View(await _context.LarryUsers.ToListAsync());
        }

        // GET: LarryUserManagement/LarryUser/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var larryUser = await _context.LarryUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (larryUser == null)
            {
                return NotFound();
            }

            return View(larryUser);
        }

        // GET: LarryUserManagement/LarryUser/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LarryUserManagement/LarryUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,Comment")] LarryUser larryUser)
        {
            if (ModelState.IsValid)
            {
                larryUser.Id = Guid.NewGuid();
                _context.Add(larryUser);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(larryUser);
        }

        // GET: LarryUserManagement/LarryUser/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var larryUser = await _context.LarryUsers.FindAsync(id);
            if (larryUser == null)
            {
                return NotFound();
            }
            return View(larryUser);
        }

        // POST: LarryUserManagement/LarryUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserId,Comment")] LarryUser larryUser)
        {
            if (id != larryUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(larryUser);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LarryUserExists(larryUser.Id))
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
            return View(larryUser);
        }

        // GET: LarryUserManagement/LarryUser/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var larryUser = await _context.LarryUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (larryUser == null)
            {
                return NotFound();
            }

            return View(larryUser);
        }

        // POST: LarryUserManagement/LarryUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var larryUser = await _context.LarryUsers.FindAsync(id);
            _context.LarryUsers.Remove(larryUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LarryUserExists(Guid id)
        {
            return _context.LarryUsers.Any(e => e.Id == id);
        }
    }
}
