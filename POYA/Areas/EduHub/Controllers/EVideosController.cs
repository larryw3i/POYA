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
    public class EVideosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EVideosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHub/EVideos
        public async Task<IActionResult> Index()
        {
            return View(await _context.EVideo.ToListAsync());
        }

        // GET: EduHub/EVideos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eVideo = await _context.EVideo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eVideo == null)
            {
                return NotFound();
            }

            return View(eVideo);
        }

        // GET: EduHub/EVideos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHub/EVideos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UseFileId,UserId,Title,DOPublish")] EVideo eVideo)
        {
            if (ModelState.IsValid)
            {
                eVideo.Id = Guid.NewGuid();
                _context.Add(eVideo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eVideo);
        }

        // GET: EduHub/EVideos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eVideo = await _context.EVideo.FindAsync(id);
            if (eVideo == null)
            {
                return NotFound();
            }
            return View(eVideo);
        }

        // POST: EduHub/EVideos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UseFileId,UserId,Title,DOPublish")] EVideo eVideo)
        {
            if (id != eVideo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eVideo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EVideoExists(eVideo.Id))
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
            return View(eVideo);
        }

        // GET: EduHub/EVideos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eVideo = await _context.EVideo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eVideo == null)
            {
                return NotFound();
            }

            return View(eVideo);
        }

        // POST: EduHub/EVideos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eVideo = await _context.EVideo.FindAsync(id);
            _context.EVideo.Remove(eVideo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EVideoExists(Guid id)
        {
            return _context.EVideo.Any(e => e.Id == id);
        }
    }
}
