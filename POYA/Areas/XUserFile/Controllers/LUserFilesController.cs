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
using POYA.Areas.XUserFile.Models;
using POYA.Data;
using POYA.Models;
using POYA.Unities.Helpers;

namespace POYA.Areas.XUserFile.Controllers
{

    [Area("XUserFile")]
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
        private readonly ILogger<Program> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly MimeHelper _mimeHelper;
        public LUserFilesController(
            MimeHelper mimeHelper,
            IAntiforgery antiforgery,
            ILogger<Program> logger,
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

        /*
        [ActionName("GetSharedImages")]
        public async Task<IActionResult> Index()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _Extensions = _mimeHelper.GetExtensions("image", _hostingEnv);
            var _LUserFiles = await _context.LUserFile.Where(p=>p.UserId==UserId_ && p.IsLegal).ToListAsync();
            _LUserFiles = _LUserFiles.Where(p => _Extensions.Contains(p.Name.Split('.').LastOrDefault())).ToList();
            return View("GetSharedImages", _LUserFiles);
        }
        */

        // GET: LUserFiles
        public async Task<IActionResult> Index(Guid? InDirId)
        {
            #region SHARING
            /*
            var _InDirId = await _context.LSharings.Where(p => p.Id == InDirId).Select(p => p.LUserFileOrDirId).FirstOrDefaultAsync();
            var _SubSharingTemp = TempData[nameof(SubSharingTemp)] as SubSharingTemp;
            TempData.Keep();
            if (_SubSharingTemp != null)
            {
                _InDirId = _InDirId == Guid.Empty ? _SubSharingTemp.SubSharings.Where(p => p.NewId == InDirId).Select(o => o.OriginalId).FirstOrDefault() : _InDirId;
            }
            var IsShared = false;
            if (_InDirId != Guid.Empty && _InDirId!=null)
            {
                IsShared = true;
                InDirId = _InDirId;
            }
            */
            #endregion
            /////////////// IF IS SHARED.. .
            InDirId = InDirId ?? Guid.Empty;
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            #region REBARBATIVE INITIALIZATION
            if (!Directory.Exists(_x_DOVEHelper.AvatarStoragePath(_hostingEnv)))
            {
                Directory.CreateDirectory(_x_DOVEHelper.AvatarStoragePath(_hostingEnv));
            }

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
            #endregion

            var LUserFile_ = await _context.LUserFile
                .Where(p => p.UserId == UserId_ && p.InDirId == InDirId && !string.IsNullOrWhiteSpace(p.MD5))
                .OrderBy(p => p.DOCreate).ToListAsync();
            var LUserFileIds = LUserFile_.Select(p => p.Id);

            #region VIEWDATA
            ViewData[nameof(InDirId)] = InDirId;
            ViewData["InDirName"] =  (await _context.LDir.Where(p => p.Id == InDirId).Select(p => p.Name).FirstOrDefaultAsync()) ?? "root";
            ViewData["LastDirId"] =  InDirId == Guid.Empty ? InDirId
                : await _context.LDir.Where(p => p.Id == InDirId && p.UserId == UserId_).Select(p => p.InDirId).FirstOrDefaultAsync();
            var _LDirs=await _context.LDir.Where(p =>  p.UserId == UserId_ && p.InDirId == InDirId && !LUserFileIds.Contains(p.Id))
                .ToListAsync();
            ViewData["LDirs"] = _LDirs;
            ViewData["_Path"] = _x_DOVEHelper.GetInPathOfFileOrDir(context: _context,InDirId: InDirId??Guid.Empty);
            //  TempData["Hello"] = "Hello!!!!!!";
            #endregion

            return View(LUserFile_);
        }

        // GET: LUserFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //  Console.WriteLine(">>>>>>>>>>>>>"+TempData["Hello"]);

            var lUserFile = await _context.LUserFile.FirstOrDefaultAsync(m => m.Id == id);
            if (lUserFile == null)
            {
                var _Id_ = await _context.LSharings.Where(p => p.Id == id).Select(p => p.LUserFileOrDirId).FirstOrDefaultAsync();
                lUserFile = await _context.LUserFile.FirstOrDefaultAsync(p => p.Id == _Id_);
                if (lUserFile == null)
                {
                    return NotFound();
                }
            }

            lUserFile.ContentType = _mimeHelper.GetMimes(lUserFile.Name, _hostingEnv).Last();
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
                await _context.LFile.AddAsync(new LFile
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
            #region     //
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

            #region MOVE AND COPY
            var AllUserSubDirs = await _context.LDir.Where(p => p.Id != Guid.Empty && p.UserId == UserId_).ToListAsync();

            lUserFile.UserAllSubDirSelectListItems = new List<SelectListItem>() { new SelectListItem {  Text="root/",Value=Guid.Empty.ToString()} };
            //  lUserFile.UserAllSubDirSelectListItems.Add(new SelectListItem {  Text="root/",Value=Guid.Empty.ToString()});
            lUserFile.UserAllSubDirSelectListItems.AddRange( AllUserSubDirs.Select(p => new SelectListItem { Text = $"{_x_DOVEHelper.GetInPathOfFileOrDir(_context, p.InDirId)}{p.Name}", Value = p.Id.ToString() }).OrderBy(p=>p.Text).ToList());
            lUserFile.CopyMoveSelectListItems = new List<SelectListItem>() {
                new SelectListItem{Text="Rename",Value="0",Selected=true},
                new SelectListItem{  Text="Also copy", Value="1"},
                new SelectListItem{Text="Also move", Value="2"}
            };
            #endregion

           

            return View(lUserFile);
        }

        // POST: LUserFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind("Id,Name,CopyMove,InDirId,IsShared,SharingCode")] LUserFile lUserFile)
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


                    if (lUserFile.InDirId!=Guid.Empty && !await _context.LDir.AnyAsync(p => p.Id == lUserFile.InDirId && p.UserId == UserId_))
                    {
                        ModelState.AddModelError(nameof(lUserFile.InDirId), _localizer["Sorry! the directory can't be found"]);
                        return View(lUserFile);
                    }

                    #region COPY
                    if (lUserFile.CopyMove == CopyMove.Copy)
                    {
                        await _context.LUserFile.AddAsync(
                            new LUserFile {
                                InDirId = lUserFile.InDirId, MD5 = _lUserFile.MD5, Name = _lUserFile.Name, UserId = UserId_, Id = Guid.NewGuid()
                            });
                        await _context.SaveChangesAsync();
                        InDirId = lUserFile.InDirId;
                    }
                    #endregion

                    #region MOVE
                    else if (lUserFile.CopyMove == CopyMove.Move)
                    {
                        await _context.SaveChangesAsync();
                        InDirId = lUserFile.InDirId;
                    }
                    #endregion

                    #region RENAME ONLY
                    else
                    {
                        _lUserFile.Name = lUserFile.Name;
                        #region
                        //  _lUserFile.ContentType = _mimeHelper.GetMime(lUserFile.Name, _hostingEnv).Last();
                        //  _lUserFile.ContentType = _mimeHelper.GetMime(lUserFile.Name).Last();   // new Mime().Lookup(lUserFile.Name);
                        #endregion
                        _context.Update(_lUserFile);
                        await _context.SaveChangesAsync();
                        InDirId = _lUserFile.InDirId;
                    }
                    #endregion
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
            lUserFile.ContentType = _mimeHelper.GetMimes(lUserFile.Name,_hostingEnv).Last();
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


        /// <summary>
        /// Get a file, it is a user's file or a file is shared
        /// </summary>
        /// <param name="id">The <see cref="LUserFile"/> id or the sharing id of <see cref="LSharing"/> </param>
        /// <param name="LSharingId">The <see cref="LSharing"/> id should be passed if you get a file in shared directory</param>
        /// <returns></returns>
        public async Task<IActionResult> GetFile(Guid? id, Guid? LSharingId)
        {
            if (id == null)
            {
                return NoContent();
            }
            //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _LUserFile = await _context.LUserFile.Select(p => new { p.MD5, p.Id, p.Name, p.UserId })
                .FirstOrDefaultAsync(p => (p.Id == id && p.UserId == _UserId) );
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
            return File(FileBytes, _mimeHelper.GetMimes(_LUserFile.Name,_hostingEnv).Last(), _LUserFile.Name, true);
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
