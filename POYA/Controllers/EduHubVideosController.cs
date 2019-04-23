using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using POYA.Data;
using POYA.Models;

namespace POYA.Controllers
{
    public class EduHubVideosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EduHubVideosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: EduHubVideos
        public async Task<IActionResult> Index()
        {
            return View(await _context.EduHubVideo.ToListAsync());
        }

        // GET: EduHubVideos/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubVideo = await _context.EduHubVideo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eduHubVideo == null)
            {
                return NotFound();
            }

            return View(eduHubVideo);
        }

        // GET: EduHubVideos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EduHubVideos/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UseFileId,UserId,Title,DOPublish")] EduHubVideo eduHubVideo)
        {
            if (ModelState.IsValid)
            {
                eduHubVideo.Id = Guid.NewGuid();
                _context.Add(eduHubVideo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eduHubVideo);
        }

        // GET: EduHubVideos/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubVideo = await _context.EduHubVideo.FindAsync(id);
            if (eduHubVideo == null)
            {
                return NotFound();
            }
            return View(eduHubVideo);
        }

        // POST: EduHubVideos/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UseFileId,UserId,Title,DOPublish")] EduHubVideo eduHubVideo)
        {
            if (id != eduHubVideo.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eduHubVideo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EduHubVideoExists(eduHubVideo.Id))
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
            return View(eduHubVideo);
        }

        // GET: EduHubVideos/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eduHubVideo = await _context.EduHubVideo
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eduHubVideo == null)
            {
                return NotFound();
            }

            return View(eduHubVideo);
        }

        // POST: EduHubVideos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var eduHubVideo = await _context.EduHubVideo.FindAsync(id);
            _context.EduHubVideo.Remove(eduHubVideo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EduHubVideoExists(Guid id)
        {
            return _context.EduHubVideo.Any(e => e.Id == id);
        }
    }
}
