using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using Ganss.XSS;
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
using POYA.Areas.EduHub.Models;
using POYA.Areas.XUserFile.Controllers;
using POYA.Areas.XUserFile.Models;
using POYA.Data;
using POYA.Unities.Helpers;
using X.PagedList;

namespace POYA.Areas.EduHub.Controllers
{
    [Area("EduHub")]
    [Authorize]
    public class EArticlesController : Controller
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
        private readonly ILogger<EArticlesController> _logger;
        private readonly HtmlSanitizer _htmlSanitizer;
        private readonly MimeHelper _mimeHelper;
        private readonly XUserFileHelper _xUserFileHelper;
        private readonly string _eArticleCategoryFilePath;
        private readonly Regex _unicode2StringRegex;
        public EArticlesController(
            MimeHelper mimeHelper,
            HtmlSanitizer htmlSanitizer,
            ILogger<EArticlesController> logger,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
           IEmailSender emailSender,
           UserManager<IdentityUser> userManager,
           ApplicationDbContext context,
           IHostingEnvironment hostingEnv,
           IStringLocalizer<Program> localizer)
        {
            _htmlSanitizer = htmlSanitizer;
            _hostingEnv = hostingEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
            _mimeHelper = mimeHelper;
            _xUserFileHelper = new XUserFileHelper();
            _eArticleCategoryFilePath = _hostingEnv.ContentRootPath + $"/Data/LAppDoc/earticle_category.csv";
            _unicode2StringRegex = new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }
        #endregion

        // GET: EduHub/EArticles
        [AllowAnonymous]
        public async Task<IActionResult> Index(bool? IsIndividual, int? SortBy, int _page = 1)
        {
            if (IsIndividual == true && !_signInManager.IsSignedIn(User))
            {
                return RedirectToPage(pageName: "/Account/Login", routeValues: new { area = "Identity", ReturnUrl = Request.Path });
            }

            var UserId_ = _userManager.GetUserAsync(User)?.GetAwaiter().GetResult()?.Id ?? string.Empty;

            #region CONTRAST IsIndividual
            var _IsIndividual = TempData[nameof(IsIndividual)];
            TempData.Keep();
            if (IsIndividual == null)
            {
                if (!string.IsNullOrWhiteSpace(_IsIndividual?.ToString()))
                {
                    IsIndividual = (bool)_IsIndividual;
                }
                else
                {
                    IsIndividual = false;
                }
            }
            #endregion


            #region CONTRAST SORTBY
            if (SortBy == null)
            {
                var _SortBy = (int)(TempData[nameof(SortBy)] ?? 1);
                TempData.Keep();
                SortBy = _SortBy;
            }
            #endregion

            var _EArticle = _context.EArticle.Where(p => IsIndividual == true ? (p.UserId == UserId_) : true)
               .OrderBy(p => p.DOPublishing);
            if (IsIndividual == false)
            {
                var _User = await _context.Users.Where(p => p.EmailConfirmed).Select(p => new { p.UserName, p.Id }).ToListAsync();
                await _EArticle.ForEachAsync(p =>
                {
                    p.UserName = _User.FirstOrDefault(o => o.Id == p.UserId)?.UserName;
                });
            }
            var _EArticleUserReadRecords = await _context.EArticleUserReadRecords.ToListAsync();
            await _EArticle.ForEachAsync(p =>
            {
                p.ReaderCount = _EArticleUserReadRecords.Where(o => o.EArticleId == p.Id).Count();
            });
            if (SortBy == (int)EArticleSortBy.Buzz)
            {
                _EArticle = _EArticle.OrderByDescending(p => p.ReaderCount);
            }
            else
            {
                _EArticle = _EArticle.OrderByDescending(p => p.DOPublishing);
            }
            var _EArticlePagedList = _EArticle.ToPagedList(_page, 8);
            var _EArticlePagedListIDs = _EArticlePagedList.Select(p => p.Id);
            var _EArticleFiles = await _context.EArticleFiles
                .Where(p => p.IsEArticleVideo && _EArticlePagedListIDs.Contains(p.EArticleId)).ToListAsync();
            ViewData["EArticles"] = _EArticlePagedList;
            //  ViewData[nameof(IsIndividual)] = IsIndividual;
            ViewData["UserId"] = UserId_;
            TempData[nameof(_page)] = _page;
            TempData[nameof(IsIndividual)] = IsIndividual;
            ViewData[nameof(IsIndividual)] = IsIndividual;
            TempData[nameof(SortBy)] = SortBy;
            ViewData[nameof(SortBy)] = SortBy;
            InitFileExtension(_EArticleFiles);

            ViewData[nameof(EArticleFile)] = _EArticleFiles;
            //  ViewData[nameof(IsIndividual)] = IsIndividual;
            return View();
        }

        // GET: EduHub/EArticles/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User)?.GetAwaiter().GetResult()?.Id ?? string.Empty;
            var eArticle = await _context.EArticle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eArticle == null)
            {
                return NotFound();
            }

            //  ViewData["Click"] = await _context.EArticleClicks.Where(p => p.EArticleId == eArticle.Id).CountAsync();
            eArticle.ClickCount += 1;
            ViewData["UserRead"] = await _context.EArticleUserReadRecords.Where(p => p.EArticleId == eArticle.Id).Select(p => p.UserId)
                .Distinct()
                .CountAsync();
            ViewData[nameof(UserId_)] = UserId_;
            if (!string.IsNullOrWhiteSpace(UserId_) && !await _context.EArticleUserReadRecords.AnyAsync(p => p.UserId == UserId_ && p.EArticleId == eArticle.Id))
            {
                await _context.EArticleUserReadRecords.AddAsync(new EArticleUserReadRecord { EArticleId = eArticle.Id, UserId = UserId_ });
            }
            await _context.SaveChangesAsync();
            var _EArticleFiles = await _context.EArticleFiles.Where(p => p.EArticleId == id).ToListAsync();
            InitFileExtension(_EArticleFiles);
            ViewData["EArticleFiles"] = _EArticleFiles;
            var Categories = GetCategories();   //  .FirstOrDefault(p => p.Id == eArticle.CategoryId);
            var Category = Categories.FirstOrDefault(p => p.Id == eArticle.CategoryId);
            var CategoryCode = Category.Code.Substring(0, 3);
            ViewData["Category"] = $" {_localizer[Categories.FirstOrDefault(p => p.Code == CategoryCode).Name]} > {_localizer[Category.Name]} >  {eArticle.AdditionalCategory}";   //  csv.GetRecords<LEArticleCategory>().ToList();
            return View(eArticle);
        }

        // GET: EduHub/EArticles/Create
        public async Task<IActionResult> Create()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _LUserFile = await _context.LUserFile.Where(p => p.UserId == UserId_).ToListAsync();  //<<<<<<<<

            var _EArticle = new EArticle();
            InitSelectListItem(_EArticle);
            return View(_EArticle);
        }

        // POST: EduHub/EArticles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("Id,Title,Content,LVideos,LAttachments,CategoryId,AdditionalCategory,ComplexityRank")][FromForm]EArticle eArticle)
        {
            if (ModelState.IsValid)
            {
                if (eArticle.Title.Length < 2 || eArticle.Content.Length < 20)
                {

                    ViewData["EArticleFiles"] = await _context.EArticleFiles.Where(p => p.EArticleId == eArticle.Id).ToListAsync();
                    eArticle.LAttachments = null;
                    eArticle.LVideos = null;
                    return View(eArticle);
                }
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var _EArticleFiles = new List<EArticleFile>();
                //  eArticle.Id = Guid.NewGuid();
                eArticle.UserId = UserId_;
                eArticle.Content = _htmlSanitizer.Sanitize(eArticle.Content);

                await _context.AddAsync(eArticle);


                //  _context.Add(eArticle);
                #region     SAVE_FILES

                await SaveArticleFilesAsync(eArticle);

                /*
                if (eArticle.LAttachments?.Count() > 0)
                {
                    foreach (var i in eArticle.LAttachments)
                    {
                        var MD5_ = await _xUserFileHelper.LWriteBufferToFileAsync(_hostingEnv, i);
                        _EArticleFiles.Add(new EArticleFile { EArticleId = eArticle.Id, FileName = System.IO.Path.GetFileName(i.FileName), FileMD5 = MD5_, IsEArticleVideo = false });
                    }
                }
                if (eArticle.LVideos?.Count() > 0)
                {
                    foreach (var i in eArticle.LVideos)
                    {
                        var MD5_ = await _xUserFileHelper.LWriteBufferToFileAsync(_hostingEnv, i);
                        _EArticleFiles.Add(new EArticleFile { EArticleId = eArticle.Id, FileName = System.IO.Path.GetFileName(i.FileName), FileMD5 = MD5_, IsEArticleVideo = true });
                    }
                }
                await _context.EArticleFiles.AddRangeAsync(_EArticleFiles);
                */
                #endregion
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(eArticle);
        }

        // GET: EduHub/EArticles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            TempData.Keep();
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var eArticle = await _context.EArticle.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId_);
            if (eArticle == null)
            {
                return NotFound();
            }
            ViewData["EArticleFiles"] = await _context.EArticleFiles.Where(p => p.EArticleId == id).ToListAsync();
            //  eArticle.VideoSharedCodeSelectListItems = await GetVideoSharedCodeSelectListItemsForUser();

            InitSelectListItem(eArticle);
            return View(eArticle);
        }

        // POST: EduHub/EArticles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,
            [Bind("Id,Title,Content,LVideos,LAttachments,CategoryId,AdditionalCategory,ComplexityRank")] EArticle eArticle)
        {
            if (id != eArticle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    var _EArticle = await _context.EArticle.Where(p => p.Id == eArticle.Id && p.UserId == UserId_).FirstOrDefaultAsync();

                    #region UPDATE
                    _EArticle.Content = _htmlSanitizer.Sanitize(eArticle.Content);
                    _EArticle.DOUpdating = DateTimeOffset.Now;
                    //  _EArticle.VideoSharedCode = eArticle.VideoSharedCode;
                    _EArticle.Title = eArticle.Title;
                    _EArticle.ComplexityRank = eArticle.ComplexityRank;
                    _EArticle.CategoryId = eArticle.CategoryId;
                    _EArticle.AdditionalCategory = eArticle.AdditionalCategory;
                    #endregion

                    await SaveArticleFilesAsync(eArticle);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EArticleExists(eArticle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { _page = TempData["_page"], IsIndividual = TempData["IsIndividual"] });
            }
            return View(eArticle);
        }

        // GET: EduHub/EArticles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var eArticle = await _context.EArticle
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == UserId_);
            if (eArticle == null)
            {
                return NotFound();
            }

            return View(eArticle);
        }

        // POST: EduHub/EArticles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var eArticle = await _context.EArticle.FirstOrDefaultAsync(p => p.Id == id && p.UserId == UserId_);
            _context.EArticle.Remove(eArticle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EArticleExists(Guid id)
        {
            return _context.EArticle.Any(e => e.Id == id);
        }


        #region DEPOLLUTION

        /// <summary>
        /// FROM        https://blog.csdn.net/qq_26422355/article/details/82716824
        /// THANK       https://blog.csdn.net/qq_26422355
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string Unicode2String(string source)
        {
            return _unicode2StringRegex.Replace(source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        }

        private void InitSelectListItem(EArticle eArticle)
        {
            var Categories = GetCategories();   // csv.GetRecords<LEArticleCategory>().ToList();
            Categories.ForEach(p =>
            {
                p.Name = _localizer[Unicode2String(p.Name)];
            });
            eArticle.ComplexityRankSelectListItems = new List<SelectListItem>{
                new SelectListItem { Value = "0", Text = "\u269D" },
                new SelectListItem { Value = "1", Text = "\u269D\u269D" },
                new SelectListItem { Value = "2", Text = "\u269D\u269D\u269D"  },
                new SelectListItem { Value = "3", Text = "\u269D\u269D\u269D\u269D"  }
            };

            eArticle.FirstCategorySelectListItems = Categories.Where(p => p.Code.Length == 3).Select(p => new SelectListItem
            {
                Value = p.Id.ToString() + "_" + p.Code,
                Text = p.Name
            }).ToList();
            eArticle.SecondCategorySelectListItems = Categories.Where(p => p.Code.Length == 5).Select(p => new SelectListItem
            {
                Value = p.Id.ToString() + "_" + p.Code,
                Text = p.Name
            }).ToList();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> RemoveArticleFile(Guid? id)
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            if (id == null)
            {
                return NotFound();
            }
            #region PERMISSION_CHECK
            var _EArticleId = await _context.EArticleFiles.Where(p => p.Id == id).Select(p => p.EArticleId).FirstOrDefaultAsync();
            if (!await _context.EArticle.AnyAsync(p => p.UserId == UserId_ && p.Id == _EArticleId))
            {
                return NotFound();
            }
            #endregion
            var _EArticleFile_ = await _context.EArticleFiles.FirstOrDefaultAsync(p => p.Id == id);
            _context.EArticleFiles.Remove(_EArticleFile_);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> LCheckMD5v1(IEnumerable<EArticleFileMD5> eArticleFileMD5s)
        {
            var _lMD5s = eArticleFileMD5s.ToList().Select(p => new LMD5 { FileMD5 = p.MD5, IsUploaded = false }).ToList();
            _lMD5s = _xUserFileHelper.LCheckMD5(_hostingEnv, _lMD5s);
            var _EArticleFiles_ = new List<EArticleFile>();
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            #region PERMISSION_CHECK
            var UserEArticles = await _context.EArticle.Where(p => p.UserId == UserId_).Select(p => new { p.Id, p.UserId }).ToListAsync();
            foreach (var e in eArticleFileMD5s)
            {
                if (UserEArticles.Any(q => q.Id == e.EArticleId && (q.UserId != UserId_)))
                {
                    return NotFound();
                }
            }

            #endregion
            eArticleFileMD5s.ToList().ForEach(p =>
            {
                if (_lMD5s.Any(q => q.FileMD5 == p.MD5 && q.IsUploaded))
                {
                    _EArticleFiles_.Add(new EArticleFile { EArticleId = p.EArticleId, FileMD5 = p.MD5, FileName = p.FileName, IsEArticleVideo = p.IsEArticleVideo });
                }
            });
            await _context.EArticleFiles.AddRangeAsync(_EArticleFiles_);
            await _context.SaveChangesAsync();
            return Ok(_lMD5s);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UploadEArticleImage([FromForm]IEnumerable<IFormFile> EArticleImages)
        {
            ///  MD5 should be checked first
            if (EArticleImages.Count() > 0)
            {
                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var _data = new List<string>();
                var _MD5s = await System.IO.Directory.GetFiles(X_DOVEValues.FileStoragePath(_hostingEnv)).Select(p => p.Split(new char[] { '\\', '/' }).LastOrDefault()).ToListAsync();
                foreach (var img in EArticleImages)
                {
                    var MemoryStream_ = new MemoryStream();
                    await img.CopyToAsync(MemoryStream_);
                    var FileBytes = MemoryStream_.ToArray();
                    var MD5_ = _xUserFileHelper.GetFileMD5(FileBytes);
                    var FilePath = X_DOVEValues.FileStoragePath(_hostingEnv) + MD5_;
                    //  System.IO.File.Create(FilePath);
                    if (!_MD5s.Contains(MD5_))
                    {
                        await System.IO.File.WriteAllBytesAsync(FilePath, FileBytes);
                        await _context.LFile.AddAsync(new LFile
                        {
                            MD5 = MD5_,
                            UserId = UserId_
                        });
                    }

                    var _LUserFile = new LUserFile
                    {
                        UserId = UserId_,
                        MD5 = MD5_,
                        InDirId = LValue.PublicDirId,
                        Name = img.FileName,
                        IsEArticleFile = true
                        //  ContentType = _LFilePost._LFile.ContentType ?? "text/plain"
                    };
                    _data.Add(Url.Action("GetEArticleImage", new { id = _LUserFile.Id }));
                    await _context.LUserFile.AddAsync(_LUserFile);
                }
                await _context.SaveChangesAsync();

                //  see https://www.kancloud.cn/wangfupeng/wangeditor3/335782
                return Ok(new { errno = 0, data = _data.ToArray() });
            }
            // process uploaded files
            // Don't rely on or trust the FileName property without validation.
            return Ok(new { errno = 1 });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEArticleImage(Guid? id)
        {
            if (id == null)
            {
                return NoContent();
            }
            //  var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _LUserFile = await _context.LUserFile
                .FirstOrDefaultAsync(p => p.IsEArticleFile && p.IsLegal && p.Id == id);
            if (_LUserFile == null)
            {
                return NoContent();
            }
            var _FilePath = X_DOVEValues.FileStoragePath(_hostingEnv) + _LUserFile.MD5;
            if (!System.IO.File.Exists(_FilePath))
            {
                return NoContent();
            }
            var FileBytes = await System.IO.File.ReadAllBytesAsync(_FilePath);
            return File(FileBytes, _xUserFileHelper.GetMimes(_LUserFile.Name, _hostingEnv).Last(), _LUserFile.Name, true);
        }

        private async Task<List<SelectListItem>> GetVideoSharedCodeSelectListItemsForUser()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var _LUserFile = await _context.LUserFile.Where(p => p.UserId == UserId_).ToListAsync();   //  <<<<<<<<
            var _VideoSharedCodeSelectListItems = new List<SelectListItem>() {
                new SelectListItem{  Value=Guid.Empty.ToString(), Text="Select your video file",Selected=true}
            };
            _LUserFile.ForEach(p =>
            {
                if (_xUserFileHelper.GetMimes(p.Name.Split(".").LastOrDefault(), _hostingEnv).LastOrDefault().StartsWith("video"))
                {
                    _VideoSharedCodeSelectListItems.Add(
                        new SelectListItem { Text = _x_DOVEHelper.GetInPathOfFileOrDir(_context, p.InDirId) + p.Name, Value = p.Id.ToString() }   //<<<<<<<<
                        );
                }
            });
            return _VideoSharedCodeSelectListItems;
        }

        private async Task SaveArticleFilesAsync(EArticle eArticle)
        {
            if (eArticle.LAttachments?.Count() < 1 || eArticle.LVideos?.Count() < 1) return;

            var _EArticleFiles = new List<EArticleFile>();
            var _LFiles_ = new List<LFile>();
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            if (eArticle.LAttachments?.Count() > 0)
            {
                foreach (var i in eArticle.LAttachments)
                {
                    if (i?.Length < 1) continue;
                    var MD5_ = await _xUserFileHelper.LWriteBufferToFileAsync(_hostingEnv, i);
                    _EArticleFiles.Add(new EArticleFile { EArticleId = eArticle.Id, FileName = System.IO.Path.GetFileName(i.FileName), FileMD5 = MD5_, IsEArticleVideo = false });
                    _LFiles_.Add(new LFile { UserId = UserId_, MD5 = MD5_ });
                }
            }
            if (eArticle.LVideos?.Count() > 0)
            {
                foreach (var i in eArticle.LVideos)
                {
                    if (i?.Length < 1) continue;
                    var MD5_ = await _xUserFileHelper.LWriteBufferToFileAsync(_hostingEnv, i);
                    _EArticleFiles.Add(new EArticleFile { EArticleId = eArticle.Id, FileName = System.IO.Path.GetFileName(i.FileName), FileMD5 = MD5_, IsEArticleVideo = true });
                    _LFiles_.Add(new LFile { UserId = UserId_, MD5 = MD5_ });
                }
            }
            await _context.EArticleFiles.AddRangeAsync(_EArticleFiles);
            await _context.LFile.AddRangeAsync(_LFiles_);
            return;
        }

        private List<LEArticleCategory> GetCategories()
        {
            var reader = new StreamReader(_eArticleCategoryFilePath);
            var csv = new CsvReader(reader);
            return csv.GetRecords<LEArticleCategory>().ToList();
        }

        private void InitFileExtension(List<EArticleFile> _EArticleFiles)
        {
            if (_EArticleFiles.Count > 0)
            {
                _EArticleFiles.ForEach(p =>
                {
                    if (!String.IsNullOrWhiteSpace(p.FileName))
                        p.ContentType = _xUserFileHelper.GetMimes(p.FileName, _hostingEnv).LastOrDefault();
                });
            }
        }
        #endregion
    }
}
