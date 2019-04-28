using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POYA.Data;
using POYA.Models;
using POYA.Unities.Helpers;

namespace POYA.Controllers
{
    [Authorize]
    public class LUserFilesController : Controller
    {
        #region
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<LUserFilesController> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly MimeHelper _mimeHelper;
        public LUserFilesController(
            MimeHelper mimeHelper,
            IAntiforgery antiforgery,
            ILogger<LUserFilesController> logger,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
           IEmailSender emailSender,
           UserManager<IdentityUser> userManager,
           ApplicationDbContext context,
           IHostingEnvironment hostingEnv,
           IStringLocalizer<Program> localizer)
        {
            _hostingEnv = hostingEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
            _mimeHelper = mimeHelper;
        }
        #endregion

        // GET: LUserFiles
        public async Task<IActionResult> Index(Guid? InDirId)
        {
            InDirId = InDirId ?? Guid.Empty;
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var _FileNames = new DirectoryInfo(_x_DOVEHelper.FileStoragePath(_hostingEnv)).GetFiles().Select(p => p.Name);
            //  Console.WriteLine("FileName >> "+JsonConvert.SerializeObject(_FileNames));
            var _LFiles = await _context.LFile.ToListAsync();
            var _LUserFiles = await _context.LUserFile.ToListAsync();
            //  Console.WriteLine("File >> "+JsonConvert.SerializeObject(_LFiles));
            _LFiles.ForEach(f =>
            {
                if (!_FileNames.Contains(f.MD5))
                {
                    _context.LFile.Remove(f);
                }
            });
            _LUserFiles.ForEach(f =>
            {
                if (!_FileNames.Contains(f.MD5))
                {
                    _context.LUserFile.Remove(f);
                }
            });
            await _context.SaveChangesAsync();

            var LUserFile_ = await _context.LUserFile.Where(p => p.UserId == UserId_ && p.InDirId == InDirId).OrderBy(p => p.DOGenerating).ToListAsync();
            var InDirName = (await _context.LDir.Where(p => p.Id == InDirId).Select(p => p.Name).FirstOrDefaultAsync()) ?? "root";
            var _Path = string.Empty;   //InDirName;
            //  LUserFile_.ForEach(m => { m.InDirName = InDirName; });
            ViewData[nameof(InDirName)] = InDirName;
            ViewData[nameof(InDirId)] = InDirId;
            ViewData["LastDirId"] = InDirId == Guid.Empty ? InDirId
                : await _context.LDir.Where(p => p.Id == InDirId && p.UserId==UserId_).Select(p => p.InDirId).FirstOrDefaultAsync();
            var LDirs = await _context.LDir.Where(p => p.InDirId == InDirId && p.UserId == UserId_).ToListAsync();
            ViewData[nameof(LDirs)] = LDirs;
            ViewData[nameof(_Path)] = _x_DOVEHelper.GetFullPathOfFileOrDir(context: _context,InDirId: InDirId??Guid.Empty);    //    $"root/{_Path}";
            return View(LUserFile_);
        }

        // GET: LUserFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile.FirstOrDefaultAsync(m => m.Id == id);
            if (lUserFile == null)
            {
                return NotFound();
            }

            lUserFile.ContentType = _mimeHelper.GetMime(lUserFile.Name, _hostingEnv).Last();
            return View(lUserFile);
        }

        // GET: LUserFiles/Create
        public IActionResult Create(Guid? InDirId)
        {
            ViewData[nameof(InDirId)] = InDirId ?? Guid.Empty;
            return View();
        }

        // POST: LUserFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm]LFilePost _LFilePost)
        {
            //  [Bind("Id,MD5,UserId,SharedCode,DOGenerating,Name,InDirId")] LUserFile lUserFile)
            if (_LFilePost._LFile.Length > 0)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var MemoryStream_ = new MemoryStream();
                await _LFilePost._LFile.CopyToAsync(MemoryStream_);
                var FileBytes = MemoryStream_.ToArray();
                var MD5_ = _x_DOVEHelper.GetFileMD5(FileBytes);
                var FilePath = _x_DOVEHelper.FileStoragePath(_hostingEnv) + MD5_;
                //  System.IO.File.Create(FilePath);
                await System.IO.File.WriteAllBytesAsync(FilePath, FileBytes);
                await _context.LFile.AddAsync(new Models.LFile
                {
                    MD5 = MD5_,
                    UserId = UserId_
                });
                await _context.LUserFile.AddAsync(new LUserFile
                {
                    UserId = UserId_,
                    MD5 = MD5_,
                    InDirId = _LFilePost.InDirId,
                    Name = _LFilePost._LFile.FileName,
                    //  ContentType = _LFilePost._LFile.ContentType ?? "text/plain"
                });
                await _context.SaveChangesAsync();
                return Ok(_LFilePost.Id);
            }
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return Ok();
            #region
            /*
            if (ModelState.IsValid)
            {
                lUserFile.Id = Guid.NewGuid();
                _context.Add(lUserFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            */
            //      return Ok();     //   View(lUserFile);
            #endregion
        }

        // GET: LUserFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lUserFile = await _context.LUserFile.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId_);
            if (lUserFile == null)
            {
                return NotFound();
            }
            var AllUserSubDirs = await _context.LDir.Where(p => p.Id != Guid.Empty && p.UserId == UserId_).ToListAsync();

            #region MOVE AND COPY
            lUserFile.UserAllSubDirSelectListItems = new List<SelectListItem>();
            //  lUserFile.UserAllSubDirSelectListItems.Add(new SelectListItem {  Text="root/",Value=Guid.Empty.ToString()});
            lUserFile.UserAllSubDirSelectListItems.AddRange( AllUserSubDirs.Select(p => new SelectListItem { Text = $"{_x_DOVEHelper.GetFullPathOfFileOrDir(_context, p.InDirId)}{p.Name}", Value = p.Id.ToString() }).OrderBy(p=>p.Text).ToList());
            lUserFile.CopyOrMoveSelectListItems = new List<SelectListItem>() {
                new SelectListItem{Text="Rename only",Value="0",Selected=true},
                new SelectListItem{  Text="Copy to", Value="1"},
                new SelectListItem{Text="Move to", Value="2"}
            };
            #endregion

            return View(lUserFile);
        }

        // POST: LUserFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,CopyOrMove,InDirId")] LUserFile lUserFile)
        {
            if (id != lUserFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var InDirId = Guid.Empty;
                try
                {
                    var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    var _lUserFile = await _context.LUserFile.FirstOrDefaultAsync(p => p.Id == lUserFile.Id && p.UserId == UserId_);
                    if (_lUserFile == null)
                    {
                        return NotFound();
                    }
                    if (!await _context.LDir.AnyAsync(p => p.Id == lUserFile.InDirId && p.UserId == UserId_))
                    {
                        ModelState.AddModelError(nameof(lUserFile.InDirId), "&#128557;");
                        return View(lUserFile);
                    }
                    //  Copy
                    if (lUserFile.CopyOrMove == 1)
                    {
                        await _context.LUserFile.AddAsync(
                            new LUserFile {
                                InDirId = lUserFile.InDirId, MD5 = _lUserFile.MD5, Name = _lUserFile.Name, UserId=UserId_,   Id=Guid.NewGuid()
                            });
                        await _context.SaveChangesAsync();
                        InDirId = lUserFile.InDirId;
                    }
                    //  Move
                    else if (lUserFile.CopyOrMove == 2)
                    {
                        _lUserFile.InDirId = lUserFile.InDirId;
                        await _context.SaveChangesAsync();
                        InDirId = lUserFile.InDirId;
                    }
                    else
                    {
                        _lUserFile.Name = lUserFile.Name;
                        //  _lUserFile.ContentType = _mimeHelper.GetMime(lUserFile.Name, _hostingEnv).Last();
                        //  _lUserFile.ContentType = _mimeHelper.GetMime(lUserFile.Name).Last();   // new Mime().Lookup(lUserFile.Name);
                        _context.Update(_lUserFile);
                        await _context.SaveChangesAsync();
                        InDirId = _lUserFile.InDirId;
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LUserFileExists(lUserFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { InDirId});
            }
            return View(lUserFile);
        }

        // GET: LUserFiles/Delete/5 //
        //  [HttpPost]
        //  [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var lUserFile = await _context.LUserFile.Where(p => p.Id == id && p.UserId == UserId_).FirstOrDefaultAsync();
            if (lUserFile == null)
            {
                return NotFound();
            }
            lUserFile.ContentType = _mimeHelper.GetMime(lUserFile.Name,_hostingEnv).Last();
            return View(lUserFile);
        }

        // POST: LUserFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var lUserFile = await _context.LUserFile.Where(p => p.Id == id && p.UserId == UserId_).FirstOrDefaultAsync();
            if (lUserFile == null)
            {
                return NotFound();
            }
            var InDirId = lUserFile.InDirId;
            _context.LUserFile.Remove(lUserFile);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index),new { InDirId});   //  Ok();     // 
        }

        private bool LUserFileExists(Guid id)
        {
            return _context.LUserFile.Any(e => e.Id == id);
        }

        #region DEPOLLUTION

        #region INDEX

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckMD5(ContrastMD5 _ContrastMD5)
        {
            //  Console.WriteLine(">>>>" + JsonConvert.SerializeObject(_ContrastMD5));
            //  System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(_ContrastMD5));
            var ContrastResult = new List<int>();
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            foreach (var i in _ContrastMD5.File8MD5s)
            {
                if (await _context.LFile.AnyAsync(p => p.MD5 == i.MD5))
                {
                    await _context.LUserFile.AddAsync(
                        new LUserFile {
                            InDirId = _ContrastMD5.InDirId,
                            MD5 = i.MD5,
                            Name = i.FileName,
                            UserId = UserId_ ,
                            //  ContentType =_mimeHelper.GetMime(i.FileName,_hostingEnv).Last()
                        });
                    ContrastResult.Add(i.Id);
                }
                await _context.SaveChangesAsync();
            }
            return Json(ContrastResult);
        }


        public async Task<IActionResult> GetFile(Guid? id)
        {
            if (id == null)
            {
                return NoContent();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _LUserFile = await _context.LUserFile.Select(p => new { p.MD5, p.Id, p.SharedCode, p.Name, p.UserId })
                .FirstOrDefaultAsync(p => (p.Id == id && p.UserId == _UserId) || p.SharedCode == id);
            if (_LUserFile == null)
            {
                return NoContent();
            }
            var _FilePath = _x_DOVEHelper.FileStoragePath(_hostingEnv) + _LUserFile.MD5;
            if (!System.IO.File.Exists(_FilePath))
            {
                return NoContent();
            }
            var FileBytes = await System.IO.File.ReadAllBytesAsync(_FilePath);
            return File(FileBytes, _mimeHelper.GetMime(_LUserFile.Name,_hostingEnv).Last(), _LUserFile.Name, true);
        }

        /*
        /// <summary>
        /// The new interface of Contrastting MD5,  be aimed at more simple, more effectivity
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CheckMd5()
        {
            return NoContent();
        }
        */
        #endregion
        #region CREATE

        /*
        public async Task<IActionResult> UploadFile(Guid? InDirId)
        {
            InDirId = InDirId ?? Guid.Empty;
            return NoContent();
        }

        /// <summary>
        /// The new upload file interface, be aimed at more simple, more effectivity
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UploadFile([FromForm] LUserFile lUserFile)
        {
            return NoContent();
        }
        */

        #endregion
        #region EDIT
        #endregion
        #region DELETE
        #endregion

        #endregion
    }
}
