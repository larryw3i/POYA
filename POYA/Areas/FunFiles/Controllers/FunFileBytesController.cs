using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.FunFiles.Models;
using POYA.Data;

namespace POYA.Areas.FunFiles.Controllers
{
    [Area("FunFiles")]
    [Authorize(Roles="843feb44-71d3-4e8c-bb79-b3f898aa3172")]
    public class FunFileBytesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FunFileBytesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FunFiles/FunFileBytes
        public async Task<IActionResult> Index()
        {
            return View(await _context.FunFileByte.ToListAsync());
        }

        // GET: FunFiles/FunFileBytes/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funFileByte = await _context.FunFileByte
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funFileByte == null)
            {
                return NotFound();
            }

            return View(funFileByte);
        }

        // GET: FunFiles/FunFileBytes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FunFiles/FunFileBytes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileSHA256,FirstUploaderId,DOUploading")] FunFileByte funFileByte)
        {
            if (ModelState.IsValid)
            {
                funFileByte.Id = Guid.NewGuid();
                _context.Add(funFileByte);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funFileByte);
        }

        // GET: FunFiles/FunFileBytes/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funFileByte = await _context.FunFileByte.FindAsync(id);
            if (funFileByte == null)
            {
                return NotFound();
            }
            return View(funFileByte);
        }

        // POST: FunFiles/FunFileBytes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FileSHA256,FirstUploaderId,DOUploading")] FunFileByte funFileByte)
        {
            if (id != funFileByte.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(funFileByte);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunFileByteExists(funFileByte.Id))
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
            return View(funFileByte);
        }

        // GET: FunFiles/FunFileBytes/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funFileByte = await _context.FunFileByte
                .FirstOrDefaultAsync(m => m.Id == id);
            if (funFileByte == null)
            {
                return NotFound();
            }

            return View(funFileByte);
        }

        // POST: FunFiles/FunFileBytes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var funFileByte = await _context.FunFileByte.FindAsync(id);
            _context.FunFileByte.Remove(funFileByte);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FunFileByteExists(Guid id)
        {
            return _context.FunFileByte.Any(e => e.Id == id);
        }
    }
}
