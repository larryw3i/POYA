using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.LarryUserManagement.Models;
using POYA.Data;

namespace POYA.Areas.LarryUserManagement.Controllers
{
    
    [Area("LarryUserManagement")]
    //  [Authorize(Roles="ADMINISTRATOR")]
    [Authorize( Roles = "4d4cba08-0e1d-497a-a649-45adf418835d")]
    public class LarryRolesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LarryRolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: LarryUserManagement/LarryRoles
        public async Task<IActionResult> Index()
        {
            return View(await _context.LarryRoles.ToListAsync());
        }

        // GET: LarryUserManagement/LarryRoles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var larryRole = await _context.LarryRoles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (larryRole == null)
            {
                return NotFound();
            }

            return View(larryRole);
        }

        // GET: LarryUserManagement/LarryRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LarryUserManagement/LarryRoles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoleId,Comment")] LarryRole larryRole)
        {
            if (ModelState.IsValid)
            {
                larryRole.Id = Guid.NewGuid();
                _context.Add(larryRole);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(larryRole);
        }

        // GET: LarryUserManagement/LarryRoles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var larryRole = await _context.LarryRoles.FindAsync(id);
            if (larryRole == null)
            {
                return NotFound();
            }
            return View(larryRole);
        }

        // POST: LarryUserManagement/LarryRoles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,RoleId,Comment")] LarryRole larryRole)
        {
            if (id != larryRole.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(larryRole);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LarryRoleExists(larryRole.Id))
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
            return View(larryRole);
        }

        // GET: LarryUserManagement/LarryRoles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var larryRole = await _context.LarryRoles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (larryRole == null)
            {
                return NotFound();
            }

            return View(larryRole);
        }

        // POST: LarryUserManagement/LarryRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var larryRole = await _context.LarryRoles.FindAsync(id);
            _context.LarryRoles.Remove(larryRole);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LarryRoleExists(Guid id)
        {
            return _context.LarryRoles.Any(e => e.Id == id);
        }


        #region     DEPOLLUTION
        
        public IActionResult GetAllRoles()
        {
            return NoContent();
        }

        #endregion
    }
}
