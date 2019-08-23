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
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POYA.Areas.EduHub.Models;
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
        //  private readonly MimeHelper _mimeHelper;
        private readonly XUserFileHelper _xUserFileHelper;
        public LUserFilesController(
            //  MimeHelper mimeHelper,
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
            //  _mimeHelper = mimeHelper;
            _xUserFileHelper = new XUserFileHelper();

        }
        #endregion


        // GET: LUserFiles
        public async Task<IActionResult> Index(Guid? InDirId)
        {

            #region REBARBATIVE INITIALIZATION
            if (!Directory.Exists(X_DOVEValues.AvatarStoragePath(_hostingEnv)))
            {
                Directory.CreateDirectory(X_DOVEValues.AvatarStoragePath(_hostingEnv));
            }

            var _FileNames = new DirectoryInfo(X_DOVEValues.FileStoragePath(_hostingEnv)).GetFiles().Select(p => p.Name);
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

            var _InDirName = "root";
            var _LastDirId = Guid.Empty;
            var _LDirs = new List<LDir>();
            var _Path = string.Empty;

            InDirId = InDirId ?? Guid.Empty;
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            #region INITIAL_SIZE
            var _UserFiles_ = await _context.LUserFile.Where(p => p.UserId == UserId_).ToListAsync();
            var UsedSpace = await GetUsedSpaceAsync(_UserFiles_);   //  0.0;
            _UserFiles_ = _UserFiles_.Where(p => p.UserId == UserId_ && p.InDirId == InDirId && !string.IsNullOrWhiteSpace(p.MD5))
                .OrderBy(p => p.DOCreate).ToList();
            _UserFiles_.ForEach(p =>
            {
                var _FileLength = new FileInfo(_xUserFileHelper.FileStoragePath(_hostingEnv) + p.MD5).Length;
                //  UsedSpace += _FileLength / (1024 * 1024);    //  MByte
                p.Size = _FileLength;
                p.OptimizedSize = _xUserFileHelper.OptimizeFileSizeShow(_FileLength);
            });
            /*
            _UserFiles_.ForEach(p=> {
                var _FileLength = new FileInfo(_xUserFileHelper.FileStoragePath(_hostingEnv) + p.MD5).Length;
                UsedSpace += _FileLength/(1024*1024);    //  MByte
                p.Size = _FileLength;
            });
            */
            #endregion

            var LUserFileIds = _UserFiles_.Select(p => p.Id);

            #region PATH
            _InDirName = (await _context.LDir.Where(p => p.Id == InDirId).Select(p => p.Name).FirstOrDefaultAsync()) ?? _InDirName;
            _LastDirId = await _context.LDir.Where(p => p.Id == InDirId && p.UserId == UserId_).Select(p => p.InDirId).FirstOrDefaultAsync();
            _LDirs = await _context.LDir.Where(p => p.UserId == UserId_ && p.InDirId == InDirId && !LUserFileIds.Contains(p.Id))
                .ToListAsync();
            _Path = X_DOVEValues.GetParentsPathOfFileOrDir(context: _context, InDirId: InDirId ?? Guid.Empty);

            //  (await _context.LDir.Where(p => p.Id == InDirId).Select(p => p.Name).FirstOrDefaultAsync()) ?? "root";
            // InDirId == Guid.Empty ? InDirId
            //    : await _context.LDir.Where(p => p.Id == InDirId && p.UserId == UserId_).Select(p => p.InDirId).FirstOrDefaultAsync();
            #endregion

            #region VIEWDATA

            ViewData[nameof(_Path)] = _Path;
            ViewData[nameof(_LDirs)] = _LDirs;
            ViewData[nameof(_LastDirId)] = _LastDirId;
            ViewData[nameof(_InDirName)] = _InDirName;
            ViewData[nameof(InDirId)] = InDirId;
            ViewData["UsedSpace"] = UsedSpace;
            ViewData["OptimizedUsedSpace"] = _xUserFileHelper.OptimizeFileSizeShow(UsedSpace);
            #endregion

            return View(_UserFiles_);

        }

        #region 
        // GET: LUserFiles/Details/5
        #endregion
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lUserFile = await _context.LUserFile.FirstOrDefaultAsync(m => m.Id == id);
            var provider = new FileExtensionContentTypeProvider();
            string contentType = string.Empty;

            if (!provider.TryGetContentType(lUserFile.Name, out contentType))
            {
                contentType = "application/octet-stream";
            }

            lUserFile.ContentType = contentType ;  //   _xUserFileHelper.GetMimes(lUserFile.Name, _hostingEnv).Last();
            return View(lUserFile);
        }

        #region 
        // GET: LUserFiles/Create
        #endregion
        public IActionResult Create(Guid? InDirId)
        {
            ViewData[nameof(InDirId)] = InDirId ?? Guid.Empty;
            return View();
        }

        #region 
        // POST: LUserFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(200_000_000)]
        #endregion
        public async Task<IActionResult> Create([FromForm] LFilePost XFilePost)
        {
            //  [Bind("Id,MD5,UserId,SharedCode,DOGenerating,Name,InDirId")] LUserFile lUserFile)

            if (XFilePost.LFile.Length > 0)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var _LUserFiles = await _context.LUserFile.Where(p => p.UserId == UserId_).ToListAsync();
                if ((await GetUsedSpaceAsync(_LUserFiles))/(Math.Pow(1024,3)) >= (5 * 1024))
                {
                    return Ok(new {Status=false, Msg =_localizer[ "The storage space is used up"] });
                }
                var MemoryStream_ = new MemoryStream();
                await XFilePost.LFile.CopyToAsync(MemoryStream_);
                var FileBytes = MemoryStream_.ToArray();
                var MD5_ = _xUserFileHelper.GetFileMD5(FileBytes);
                var FilePath = X_DOVEValues.FileStoragePath(_hostingEnv) + MD5_;
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
                    InDirId = XFilePost.InDirId,
                    Name = XFilePost.LFile.FileName,
                    //  ContentType = _LFilePost._LFile.ContentType ?? "text/plain"
                });
                await _context.SaveChangesAsync();
                return Ok(new { Status=true, XFilePost.Id });
            }
            return Ok();
        }

        #region 
        // GET: LUserFiles/Edit/5
        #endregion
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

            lUserFile.UserAllSubDirSelectListItems = new List<SelectListItem>() { new SelectListItem { Text = "root/", Value = Guid.Empty.ToString() } };
            lUserFile.UserAllSubDirSelectListItems.AddRange(AllUserSubDirs.Select(p => new SelectListItem { Text = $"{X_DOVEValues.GetParentsPathOfFileOrDir(_context, p.InDirId)}{p.Name}", Value = p.Id.ToString() }).OrderBy(p => p.Text).ToList());
            lUserFile.CopyMoveSelectListItems = new List<SelectListItem>() {
                new SelectListItem { Text =_localizer[ "Rename"], Value = "0", Selected = true },
                new SelectListItem { Text =_localizer[ "Also copy"], Value = "1" },
                new SelectListItem { Text =_localizer[ "Also move"], Value = "2" }
            };
            #endregion

            return View(lUserFile);
        }

        #region 
        // POST: LUserFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        #endregion
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

                    if (lUserFile.InDirId != Guid.Empty && !await _context.LDir.AnyAsync(p => p.Id == lUserFile.InDirId && p.UserId == UserId_))
                    {
                        ModelState.AddModelError(nameof(lUserFile.InDirId), _localizer["Sorry! the directory can't be found"]);
                        return View(lUserFile);
                    }

                    #region COPY
                    if (lUserFile.CopyMove == CopyMove.Copy)
                    {
                        await _context.LUserFile.AddAsync(
                            new LUserFile
                            {
                                InDirId = lUserFile.InDirId,
                                MD5 = _lUserFile.MD5,
                                Name = _lUserFile.Name,
                                UserId = UserId_,
                                Id = Guid.NewGuid()
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
                return RedirectToAction(nameof(Index), new { InDirId });
            }
            return View(lUserFile);
        }

        #region 
        // GET: LUserFiles/Delete/5 //
        //  [HttpPost]
        //  [ValidateAntiForgeryToken]
        #endregion
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
            var provider = new FileExtensionContentTypeProvider();
            string contentType = string.Empty;
            if (!provider.TryGetContentType(lUserFile.Name, out contentType))
            {
                contentType = "application/octet-stream";
            }

            lUserFile.ContentType = contentType;    //   _xUserFileHelper.GetMimes(lUserFile.Name, _hostingEnv).Last();
            return View(lUserFile);
        }

        #region 
        // POST: LUserFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        #endregion
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
            return RedirectToAction(nameof(Index), new { InDirId }); //  Ok();     // 
        }

        private bool LUserFileExists(Guid id)
        {
            return _context.LUserFile.Any(e => e.Id == id);
        }

        #region DEPOLLUTION

        #region 
        /// <summary>
        /// Get the used storge space of user(in byte)
        /// </summary>
        /// <param name="_UserFiles_"></param>
        /// <returns></returns>
        #endregion
        private async Task<long> GetUsedSpaceAsync(List<LUserFile> _UserFiles_)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            _UserFiles_ = await _context.LUserFile.Where(p => p.UserId == UserId_).ToListAsync();
            var UsedSpace = (long)0;
            _UserFiles_.ForEach(p =>
            {
                var _FileLength = new FileInfo(_xUserFileHelper.FileStoragePath(_hostingEnv) + p.MD5).Length;
                UsedSpace += _FileLength;   //  / (1024 * 1024);    //  MByte

            });
            return UsedSpace;
        }

        #region 

        [HttpPost]
        [Authorize(Roles = "06c05751-3d63-4fe0-bee0-1f2a7226a7c2")]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> UploadEArticleHomeInfo([FromForm]UserEArticleHomeInfo userEArticleHomeInfo)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _userEArticleHomeInfo = await _context.UserEArticleHomeInfos.FirstOrDefaultAsync(p => p.UserId == UserId_);
            var _MD5= string.Empty;
            if (userEArticleHomeInfo?.CoverFile != null)
            {
                _MD5 = await _xUserFileHelper.LWriteBufferToFileAsync(_hostingEnv, userEArticleHomeInfo.CoverFile);
            }
            if (_userEArticleHomeInfo == null)
            {
                await _context.UserEArticleHomeInfos.AddAsync(new UserEArticleHomeInfo
                {
                    UserId = UserId_,
                    Comment = userEArticleHomeInfo.Comment,
                    CoverFileMD5 = _MD5,
                    Id = Guid.NewGuid(),
                    CoverFileContentType = string.IsNullOrWhiteSpace(_MD5) ?
                    "" : userEArticleHomeInfo.CoverFile.ContentType
                });
            }
            else
            {
                _userEArticleHomeInfo.Comment = string.IsNullOrWhiteSpace(userEArticleHomeInfo.Comment) ?
                    _userEArticleHomeInfo.Comment : userEArticleHomeInfo.Comment;
                _userEArticleHomeInfo.CoverFileMD5 = string.IsNullOrWhiteSpace(_MD5) ?
                    _userEArticleHomeInfo.CoverFileMD5 : _MD5;
                _userEArticleHomeInfo.CoverFileContentType = string.IsNullOrWhiteSpace(_MD5) ?
                    _userEArticleHomeInfo.CoverFileContentType : userEArticleHomeInfo.CoverFile.ContentType;
            }
            await _context.SaveChangesAsync();
            return Ok();
        }

        #region 
        /// <summary>
        /// The basic method for checking MD5
        /// </summary>
        /// <returns></returns>
        #endregion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XCheckMD5([FromForm]List<string> MD5s)
        {
            var CheckResult = new Dictionary<string, bool>();
            var UploadFileMD5s = System.IO.Directory.GetFiles(X_DOVEValues.FileStoragePath(_hostingEnv))
             .Select(p => System.IO.Path.GetFileNameWithoutExtension(p)).ToList();

            foreach (var m in MD5s)
            {
                if (UploadFileMD5s.Contains(m))
                {
                    CheckResult.Add(m, true);
                }
                CheckResult.Add(m, false);
            }

            return Json(CheckResult.Select(p => new { MD5 = p.Key, IsUploaded = p.Value }).ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckMD5(ContrastMD5 LContrastMD5)
        {
            //  Console.WriteLine(">>>>" + JsonConvert.SerializeObject(_ContrastMD5));
            //  System.Diagnostics.Debug.WriteLine(JsonConvert.SerializeObject(_ContrastMD5));
            var ContrastResult = new List<string>();
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            foreach (var i in LContrastMD5.File8MD5s)
            {
                if (await _context.LFile.AnyAsync(p => p.MD5 == i.MD5))
                {
                    await _context.LUserFile.AddAsync(
                        new LUserFile
                        {
                            InDirId = LContrastMD5.InDirId,
                            MD5 = i.MD5,
                            Name = i.FileName,
                            UserId = UserId_,
                            //  ContentType =_mimeHelper.GetMime(i.FileName,_hostingEnv).Last()
                        });
                    ContrastResult.Add(i.Id);
                }
                await _context.SaveChangesAsync();
            }
            return Json(ContrastResult);
        }

        #region 
        /// <summary>
        /// Get a file, it is a user's file or a file is shared
        /// </summary>
        /// <param name="id">The <see cref="LUserFile"/> id or the sharing id of <see cref="LSharing"/> </param>
        /// <param name="LSharingId">The <see cref="LSharing"/> id should be passed if you get a file in shared directory</param>
        /// <returns></returns>
        [AllowAnonymous]
        #endregion
        public async Task<IActionResult> GetFile(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User)?.GetAwaiter().GetResult()?.Id;

            var provider = new FileExtensionContentTypeProvider();
            string contentType = string.Empty;


            #region EARTICLE_FILE
            var _EArticleFile = await _context.EArticleFiles.FirstOrDefaultAsync(p => p.Id == id);
            if (_EArticleFile != null)
            {
                var _FilePath_ = X_DOVEValues.FileStoragePath(_hostingEnv) + _EArticleFile.FileMD5;
                if (!System.IO.File.Exists(_FilePath_))
                {
                    return NoContent();
                }
                var _FileBytes = await System.IO.File.ReadAllBytesAsync(_FilePath_);
                if (!provider.TryGetContentType(_EArticleFile.FileName, out contentType))
                {
                    contentType = "application/octet-stream";
                }

                return File(_FileBytes, contentType, _EArticleFile.FileName, true);
            }
            #endregion

            var _LUserFile = await _context.LUserFile.Select(p => new { p.MD5, p.Id, p.Name, p.UserId })
                .FirstOrDefaultAsync(p => (p.Id == id && p.UserId == _UserId));
            if (_LUserFile == null)
            {
                return NotFound();
            }
            var _FilePath = X_DOVEValues.FileStoragePath(_hostingEnv) + _LUserFile.MD5;
            if (!System.IO.File.Exists(_FilePath))
            {
                return NotFound();
            }


            if (!provider.TryGetContentType(_LUserFile.Name, out contentType))
            {
                contentType = "application/octet-stream";
            }
            var FileBytes = await System.IO.File.ReadAllBytesAsync(_FilePath);
            return File(FileBytes, contentType, _LUserFile.Name, true);
        }
        #region 

        /*
        /// <summary>
        /// The new interface of Contrastting MD5,  be aimed at more simple, more effectivity
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CheckMD5()
        {
            return NoContent();
        }
       
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

        #endregion
    }
}