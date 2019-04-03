using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
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
using POYA.Data;
using POYA.Models;
using POYA.Unities.Helpers;

namespace POYA.Controllers
{
    [Authorize]
    public class X_doveFilesController : Controller
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
        private readonly ILogger<HomeController> _logger;

        public X_doveFilesController(
            ILogger<HomeController> logger,
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
        }
        #endregion

        // GET: X_doveFile
        public async Task<IActionResult> Index(Guid? id)
        {
            id = id ?? Guid.Empty;
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            ViewData["Dirs"] = await _context.X_doveDirectories.Where(p => p.InDirId == id && p.UserId == _UserId).ToListAsync();
            ViewData["Files"] = await _context.X_doveFiles.Where(p => p.InDirId == id && p.UserId == _UserId && !p.IsDeleted)
                .Select(p => new X_doveFile
                {
                    FileBuffer = null,
                    ContentType = p.ContentType,
                    InDirId = p.InDirId,
                    DOUpload = p.DOUpload,
                    Id = p.Id,
                    Name = p.Name,
                    Size = p.Size == 0 ? _context.X_doveFiles.Where(q => q.Id == p.CoopyOfId).Select(m => m.Size).FirstOrDefault() : p.Size
                }).ToListAsync();
            TempData["DirId"] = id;
            ViewData["DirName"] = "./" + (id == Guid.Empty ? "Root" : (await _context.X_doveDirectories.Where(p => p.Id == id).Select(p => p.Name).FirstOrDefaultAsync()));
            if (id != Guid.Empty)
            {
                ViewData["InDirId"] = await _context.X_doveDirectories.Where(p => p.Id == id).Select(p => p.InDirId).FirstOrDefaultAsync();
            }
            return View();
        }

        // GET: X_doveFile/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            #region
            /*
            if (SharingCode == "" && await _context.X_doveFiles.AnyAsync(p=>p.UserId!=_UserId && p.IsShared))
            {
                var _Token = Guid.NewGuid().ToString();
                TempData["Token"] = _Token;
                ViewData["Parameters"] = new Dictionary<String, string>
                {
                    { "Id", id.ToString() },{"Token",_Token},
                };
                return View("RequestCode");
            }*/
            #endregion

            var x_doveFile = await _context.X_doveFiles.Where(p => p.UserId == _UserId && !p.IsDeleted)
                .Select(p => new X_doveFile
                {
                    Name = p.Name,
                    ContentType = p.ContentType,
                    Size = p.CoopyOfId == Guid.Empty ? p.Size : _context.X_doveFiles.Where(q => q.Id == p.CoopyOfId).Select(x => x.Size).FirstOrDefault(),
                    DOUpload = p.DOUpload,
                    Id = p.Id
                })
                .FirstOrDefaultAsync(m => m.Id == id);
            if (x_doveFile == null)
            {
                return NotFound();
            }

            return View(x_doveFile);
        }



        // GET: X_doveFile/Create
        public IActionResult Create(string DirId = "")
        {
            if (string.IsNullOrWhiteSpace(DirId)) DirId = Guid.Empty.ToString();
            TempData["DirId"] = DirId;
            ViewData["DirId"] = DirId;
            return View();
        }

        // POST: X_doveFile/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(List<IFormFile> files)
        {
            var Message = "";
            if ((await _context.X_doveFiles.Select(p => p.Size).SumAsync()) / 1073741824 > 2)
            {
                Message = _localizer["The engineer is trying to solve the problem of the size of storage space"];
                return View();
            }
            var Md5 = MD5.Create();
            var x_doveFiles = new List<X_doveFile>();
            var _DirId = TempData["DirId"]?.ToString();
            var DirId = string.IsNullOrWhiteSpace(_DirId) ? Guid.Empty : Guid.Parse(_DirId);
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            foreach (var _f in files)
            {
                var FileName = Path.GetFileName(_f.FileName);
                #region
                if (_f.Length < 1)
                {
                    Message += $"<br/>{FileName}<br/>&nbsp;&nbsp;&nbsp;&nbsp;{_localizer["Upload unsuccessfully"]}, {_localizer["it's length is 0"]}";
                }
                else
                {
                    var memoryStream = new MemoryStream();
                    var _x_doveFile = new X_doveFile
                    {
                        ContentType = _f.ContentType,
                        UserId = _UserId,
                        InDirId = DirId,
                        Name = FileName
                    };
                    await _f.CopyToAsync(memoryStream);
                    var buffer = memoryStream.ToArray();
                    var Hash = Md5.ComputeHash(buffer);
                    var CopyOfId = await _context.X_doveFiles.Where(p => p.Hash == Hash).Select(p => p.Id).FirstOrDefaultAsync();
                    if (CopyOfId != null && CopyOfId != Guid.Empty)
                    {
                        _x_doveFile.CoopyOfId = CopyOfId;
                    }
                    else
                    {
                        _x_doveFile.FileBuffer = buffer;
                        _x_doveFile.Size = _f.Length;
                        _x_doveFile.Hash = Hash;
                    }
                    x_doveFiles.Add(_x_doveFile);
                    Message += $"<br/>{FileName} >_<br/>&nbsp;&nbsp;&nbsp;&nbsp;{_localizer["Upload successfully"]}";
                }
                #endregion
            }
            try
            {
                await _context.X_doveFiles.AddRangeAsync(x_doveFiles);
                await _context.SaveChangesAsync();
            }
            catch
            {
                Message = _localizer["An error has occurred, the engineer is on his way"];
            }
            ViewData["Message"] = Message;
            ViewData["DirId"] = DirId;
            return View(nameof(Create));
        }
        #region
        /*
         * 
            long size = files.Sum(f => f.Length);

            // full path to file in temp location
            var filePath = Path.GetTempFileName();

            foreach (var formFile in files)
            {
                if (formFile.Length > 0)
                {
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            // process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size, filePath });

         * [Bind("Id,DirId,Name,ContentType,DOUpload,Size,FileBuffer")] X_doveFile x_doveFile)
    {
        if (ModelState.IsValid)
        {
            x_doveFile.Id = Guid.NewGuid();
            _context.Add(x_doveFile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(x_doveFile);
    }*/

        #endregion
        // GET: X_doveFile/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var x_doveFile = await _context.X_doveFiles.Where(p => !p.IsDeleted && p.Id == id && p.UserId == _UserId).Select(
                p => new X_doveFile
                {
                    Name = p.Name,
                    Id = p.Id
                }
                ).FirstOrDefaultAsync();
            if (x_doveFile == null)
            {
                return NotFound();
            }
            #region
            /*
            if (From == "Details")
            {
                TempData["From"]= "Details";
                //  RedirectToAction(nameof(Details),new { x_doveFile.Id});
                //  return View("Details",x_doveFile);
            }
            */
            #endregion
            return View(x_doveFile);
        }

        // POST: X_doveFile/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,IsShared")] X_doveFile x_doveFile)
        {
            if (id != x_doveFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var _x_doveFile = await _context.X_doveFiles.Where(p => p.Id == id && !p.IsDeleted && p.UserId == _UserId).FirstOrDefaultAsync();
                try
                {
                    _x_doveFile.Name = x_doveFile.Name;
                    //  _x_doveFile.IsShared = x_doveFile.IsShared;
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!X_doveFileExists(x_doveFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(actionName: "Index", routeValues: new { _x_doveFile.InDirId });
            }
            return View(x_doveFile);
        }

        // GET: X_doveFile/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var x_doveFile = await _context.X_doveFiles.Where(p => p.UserId == _UserId && !p.IsDeleted)
               .Select(p => new X_doveFile
               {
                   Name = p.Name,
                   ContentType = p.ContentType,
                   Size = p.CoopyOfId == Guid.Empty ? p.Size : _context.X_doveFiles.Where(q => q.Id == p.CoopyOfId).Select(m => m.Size).FirstOrDefault(),
                   DOUpload = p.DOUpload,
                   Id = p.Id,
                   //   IsShared=p.IsShared
               })
               .FirstOrDefaultAsync(m => m.Id == id);
            if (x_doveFile == null)
            {
                return NotFound();
            }

            return View(x_doveFile);
        }

        // POST: X_doveFile/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var x_doveFile = await _context.X_doveFiles.Where(p => p.UserId == _UserId)
               .FirstOrDefaultAsync(m => m.Id == id);
            var IsCopied = await _context.X_doveFiles.AnyAsync(p => p.CoopyOfId == x_doveFile.Id);
            if ((x_doveFile.CoopyOfId == Guid.Empty && !IsCopied) || x_doveFile.CoopyOfId != Guid.Empty)
            {
                _context.X_doveFiles.Remove(x_doveFile);
            }
            else
            {
                x_doveFile.IsDeleted = true;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(actionName: "Index", routeValues: new { x_doveFile.InDirId });
        }

        public async Task<IActionResult> GetFile(string Id = "")
        {
            if (string.IsNullOrWhiteSpace(Id)) return null;
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _id = Guid.Parse(Id);
            var _x_doveFile = await _context.X_doveFiles.Where(p => p.Id == _id && !p.IsDeleted && p.UserId == _UserId).Select(p => new { IsCopy = p.CoopyOfId != Guid.Empty, p.CoopyOfId, p.ContentType, p.Name }).FirstOrDefaultAsync();
            if (_x_doveFile == null) return null;
            var buffer = new byte[0];
            if (_x_doveFile.IsCopy)
                buffer = await _context.X_doveFiles.Where(p => p.Id == _x_doveFile.CoopyOfId).Select(p => p.FileBuffer).FirstOrDefaultAsync();
            else
                buffer = await _context.X_doveFiles.Where(p => p.Id == _id).Select(p => p.FileBuffer).FirstOrDefaultAsync();
            return File(fileContents: buffer, contentType: _x_doveFile.ContentType, fileDownloadName: _x_doveFile.Name);
        }

        [AllowAnonymous]
        public async Task<IActionResult> GetAvatar(string UserId = "")
        {
            UserId = string.IsNullOrWhiteSpace(UserId) ? _userManager.GetUserAsync(User)?.GetAwaiter().GetResult()?.Id ?? "" : UserId;

            if (string.IsNullOrWhiteSpace(UserId))
            {
                return File(fileContents: await GetDefaultAvatarByteArrayAsync(), contentType: "image/png", fileDownloadName: "chat_with_friends_ico.png");
            }
            //  return NoContent();
            var IsRegisteredUser = await _context.Users.AnyAsync(p => p.Id == UserId);
            if (!IsRegisteredUser)
            {
                return NoContent();
            }

            var Buffer_UserId = await _context.X_DoveUserInfos.Select(p => new { p.AvatarBuffer, p.UserId }).FirstOrDefaultAsync(p => p.UserId == UserId);

            if ((IsRegisteredUser && (Buffer_UserId == null)) || Buffer_UserId?.AvatarBuffer == null || Buffer_UserId?.AvatarBuffer.Length < 1)
            {
                return File(fileContents: await GetDefaultAvatarByteArrayAsync(), contentType: "image/png", fileDownloadName: "chat_with_friends_ico.png");
            }
            return File(fileContents: Buffer_UserId.AvatarBuffer, contentType: "image/png", fileDownloadName: Buffer_UserId.UserId + ".png");
        }
        private async Task<byte[]> GetDefaultAvatarByteArrayAsync()
        {
            var _fileStream = System.IO.File.OpenRead(path: _hostingEnv.WebRootPath + "\\img\\chat_with_friends_ico.png");
            var _fileMemoryStream = new MemoryStream();
            await _fileStream.CopyToAsync(_fileMemoryStream);
            return _fileMemoryStream.ToArray();   // 
        }

        [HttpPost]
        public async Task<IActionResult> UploadAvatar([FromForm]AvatarForm avatarForm)
        {
            var X_DOVE_XSRF_TOKEN = TempData["X_DOVE_XSRF_TOKEN"].ToString();
            if (!avatarForm.AvatarImgFile.ContentType.Contains("image") || X_DOVE_XSRF_TOKEN == avatarForm.X_DOVE_XSRF_TOKEN)
            {
                return BadRequest();
            }
            if (avatarForm.AvatarImgFile.Length > 1024 * 1024)
            {
                return Json(new { status = false, msg = _localizer["Oops...., the size of avatar should be less than 1M"] });
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var memoryStream = new MemoryStream();
            await avatarForm.AvatarImgFile.CopyToAsync(memoryStream);
            var AvatarBuffer = MakeCircleImage(memoryStream);//  memoryStream.ToArray();
            var _X_doveUserInfo = await _context.X_DoveUserInfos.FirstOrDefaultAsync(p => p.UserId == _UserId);

            if (_X_doveUserInfo != null && AvatarBuffer == _X_doveUserInfo.AvatarBuffer)
                return Json(new { status = true, X_DOVE_XSRF_TOKEN });

            if (_X_doveUserInfo != null)
            {
                _X_doveUserInfo.AvatarBuffer = AvatarBuffer;
            }
            else
            {
                _X_doveUserInfo = new X_doveUserInfo() { UserId = _UserId, AvatarBuffer = AvatarBuffer };
                await _context.X_DoveUserInfos.AddAsync(_X_doveUserInfo);
            }
            await _context.SaveChangesAsync();
            X_DOVE_XSRF_TOKEN = Guid.NewGuid().ToString();
            TempData["X_DOVE_XSRF_TOKEN"] = X_DOVE_XSRF_TOKEN;
            return Json(new { status = true, X_DOVE_XSRF_TOKEN });
        }

        /// <summary>
        /// PART FROM   https://www.cnblogs.com/wjshan0808/p/5909174.html
        /// THANK       https://www.cnblogs.com/wjshan0808/
        /// </summary>
        /// <param name="ImageBytes">The MemoryStream of image</param> 
        /// <returns></returns>
        private byte[] MakeCircleImage(MemoryStream ImageMemoryStream)
        {
            var img = Image.FromStream(ImageMemoryStream);
            var _min = Math.Min(img.Height, img.Width);
            var b = new Bitmap(_min, _min);
            using (var g = Graphics.FromImage(b))
            {
                g.DrawImage(image: img,
                    width: img.Width,
                    height: img.Height,
                    x: (-(img.Width - _min) / 2),
                    y: (-(img.Height - _min) / 2));
                var r = _min / 2;
                var c = new PointF(_min / 2.0F, _min / 2.0F);
                for (int h = 0; h < _min; h++)
                {
                    for (var w = 0; w < _min; w++)
                    {
                        if ((int)Math.Pow(r, 2) < ((int)Math.Pow(w * 1.0 - c.X, 2) + (int)Math.Pow(h * 1.0 - c.Y, 2)))
                        {
                            b.SetPixel(w, h, Color.Transparent);
                        }
                    }
                }
                using (var p = new Pen(Color.Transparent))
                    g.DrawEllipse(p, 0, 0, _min, _min);
            }
            var ms = new MemoryStream();
            b.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        public async Task<IActionResult> Copy(Guid Id, string FoDName, string ReturnUrl = null, bool IsFile = false)
        {
            return await Copy8MoveCommon(Id: Id, FoDName: FoDName, ReturnUrl: ReturnUrl, IsFile: IsFile, IsMove: false);
        }

    
        public async Task<IActionResult> Move(Guid Id, string FoDName, string ReturnUrl = null, bool IsFile = false)
        {
            return await Copy8MoveCommon(Id:Id, FoDName: FoDName,ReturnUrl: ReturnUrl,IsFile: IsFile,IsMove:true);
        }


        private async Task<IActionResult> Copy8MoveCommon(Guid Id, string FoDName, string ReturnUrl = null, bool IsFile = false,bool IsMove=false)
        {
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var SelectListItemsOfX_doveDirectory = await _context.X_doveDirectories.Where(p => p.UserId == _UserId).Select(p =>
                 new SelectListItem { Text = p.Name, Value = p.Id.ToString() }
            ).ToListAsync();

            ViewData[nameof(SelectListItemsOfX_doveDirectory)] = SelectListItemsOfX_doveDirectory;

            return View(viewName: nameof(Copy8Move), model: new Copy8Move { Id = Id, ReturnUrl = ReturnUrl, IsMove = IsMove, IsFile = IsFile, FoDName = FoDName });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Move(Copy8Move Copy8Move)
        {
                return await Copy8MoveCommon(Copy8Move);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Copy(Copy8Move Copy8Move)
        {
                return await Copy8MoveCommon(Copy8Move);
        }


        private async Task<IActionResult> Copy8MoveCommon(Copy8Move Copy8Move)
        {
            if (ModelState.IsValid)
            {
                var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                var IsUserDir = await _context.X_doveDirectories.AnyAsync(p => p.Id == Copy8Move.ToDirId);
                if (!IsUserDir)
                {
                    return View(viewName: nameof(Copy8Move), model: Copy8Move);
                }
                if (Copy8Move.IsFile){
                    if (Copy8Move.IsMove)
                    {
                        var _X_doveFile_ = await _context.X_doveFiles.Where(p => p.UserId == _UserId && p.Id == Copy8Move.Id).FirstOrDefaultAsync();
                        if (_X_doveFile_ == null)
                        {
                            return View(viewName: nameof(Copy8Move), model: Copy8Move);
                        }
                        _X_doveFile_.InDirId = Copy8Move.ToDirId;
                    }
                }
                else
                {
                    if (Copy8Move.IsMove)
                    {
                        var _X_doveDirectory_ = await _context.X_doveDirectories.Where(p => p.UserId == _UserId && p.Id == Copy8Move.Id).FirstOrDefaultAsync();
                        if (_X_doveDirectory_ == null)
                        {
                            return View(viewName: nameof(Copy8Move), model: Copy8Move);
                        }
                        _X_doveDirectory_.InDirId = Copy8Move.ToDirId;
                    }

                }
                await _context.SaveChangesAsync();
                return LocalRedirect(Copy8Move.ReturnUrl);
            }
            return View(viewName: nameof(Copy8Move), model: Copy8Move);
        }

        private bool X_doveFileExists(Guid id)
        {
            return _context.X_doveFiles.Any(e => e.Id == id);
        }
    }
}
