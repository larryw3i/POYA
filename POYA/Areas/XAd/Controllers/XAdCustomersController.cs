using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Ganss.XSS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POYA.Areas.EduHub.Controllers;
using POYA.Areas.XAd.Models;
using POYA.Areas.XUserFile.Controllers;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.XAd.Controllers
{
    [Area("XAd")]
    [Authorize]
    public class XAdCustomersController : Controller
    {

        #region     DI
        private readonly IHostingEnvironment _hostingEnv;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<XAdCustomersController> _logger;
        private readonly HtmlSanitizer _htmlSanitizer;
        //  private readonly MimeHelper _mimeHelper;
        private readonly XUserFileHelper _xUserFileHelper;
        public XAdCustomersController(
            //  MimeHelper mimeHelper,
            HtmlSanitizer htmlSanitizer,
            ILogger<XAdCustomersController> logger,
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
            //  _mimeHelper = mimeHelper;
            _xUserFileHelper = new XUserFileHelper();
        }
        #endregion

        #region 

        // GET: XAd/XAdCustomers
        #endregion
        public async Task<IActionResult> Index()
        {
            var _XAdCustomer = await _context.XAdCustomer.OrderByDescending(p => p.DORegistering).Take(10).ToListAsync();
            return View(_XAdCustomer);
        }

        #region 

        // GET: XAd/XAdCustomers/Details/5
        #endregion
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var xAdCustomer = await _context.XAdCustomer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xAdCustomer == null)
            {
                return NotFound();
            }
            
            xAdCustomer.UserName = await _context.Users.Where(p=>p.Id==xAdCustomer.UserId).Select(p=>p.UserName).FirstOrDefaultAsync();
            ViewData["CurrentUserId"]=UserId_;

            return View(xAdCustomer);
        }

        #region 

        // GET: XAd/XAdCustomers/Create
        #endregion
        public async Task<IActionResult> Create()
        {
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _XAdCustomer=await _context.XAdCustomer.FirstOrDefaultAsync(p=>p.UserId==UserId_);
            if(_XAdCustomer!=null){
                return RedirectToAction(nameof(Details),new{id =_XAdCustomer.Id});
            }
            return View();
        }

        #region 

        // POST: XAd/XAdCustomers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> Create([Bind("Id,Name,DORegistering,LicenseImgFiles,Address,StoreIconFile,Intro")] XAdCustomer xAdCustomer)
        {
            if (ModelState.IsValid)
            {
                if (xAdCustomer.StoreIconFile == null || xAdCustomer.LicenseImgFiles?.Count() < 3 || xAdCustomer.LicenseImgFiles.Count() > 5)
                {
                    return View(xAdCustomer);
                }
                #region ===>   FILES_VALIDATE
                var _xAdCustomerFiles = xAdCustomer.LicenseImgFiles; //  .Add(xAdCustomer.StoreIconFile);
                _xAdCustomerFiles.Add(xAdCustomer.StoreIconFile);
                if (_xAdCustomerFiles.Any(p => !p.ContentType.StartsWith("image/") || !p.FileName.Contains('.') || p.Length > 2048 * 1024 || p.Length < 1))
                {
                    return View(xAdCustomer);
                }


                #endregion

                var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

                xAdCustomer.Id = Guid.NewGuid();

                #region ===>    XADCUSTOMERLICENSE

                var _XAdCustomerLicenses = new List<XAdCustomerLicense>();

                var _FileMD5s = System.IO.Directory.GetFiles(XAdCustomerHelper.XAdCustomerLicenseImgFilePath(_hostingEnv)).Select(p => System.IO.Path.GetFileNameWithoutExtension(p));

                foreach (var f in xAdCustomer.LicenseImgFiles)
                {
                    var _memoryStream = new MemoryStream();
                    await f.CopyToAsync(_memoryStream);
                    var _bytes = _memoryStream.ToArray();
                    var _md5 = new XUserFileHelper().GetFileMD5(_bytes);

                    if (!_hostingEnv.IsDevelopment() && _FileMD5s.Contains(_md5))
                    {
                        ModelState.AddModelError(nameof(XAdCustomer.LicenseImgFiles), _localizer["The license photo is repeated"]);
                        return View(xAdCustomer);
                    }
                    if (!_FileMD5s.Contains(_md5))
                    {
                        var _FileStream = new FileStream(XAdCustomerHelper.XAdCustomerLicenseImgFilePath(_hostingEnv) + $"/{_md5}", FileMode.Create);
                        await f.CopyToAsync(_FileStream);
                        _FileStream.Close();

                    }
                    _XAdCustomerLicenses.Add(new XAdCustomerLicense { Id = Guid.NewGuid(), XAdCustomerUserId = UserId_, ImgFileMD5 = _md5, ImgFileContentType = f.ContentType });

                }
                await _context.XAdCustomerLicenses.AddRangeAsync(_XAdCustomerLicenses);

                #endregion

                #region ===>    STORE_ICON
                var _memoryStream_ = new MemoryStream();
                await xAdCustomer.StoreIconFile.CopyToAsync(_memoryStream_);
                var _bytes_ = _memoryStream_.ToArray();
                var _md5_ = new XUserFileHelper().GetFileMD5(_bytes_);

                if (!_hostingEnv.IsDevelopment() && _FileMD5s.Contains(_md5_))
                {
                    ModelState.AddModelError(nameof(XAdCustomer.LicenseImgFiles), _localizer["The store icon is repeated"]);
                    return View(xAdCustomer);
                }
                if (!_FileMD5s.Contains(_md5_))
                {
                    var _FileStream_ = new FileStream(XAdCustomerHelper.XAdCustomerLicenseImgFilePath(_hostingEnv) +
                        $"/{_md5_}",
                    FileMode.Create);
                    await xAdCustomer.StoreIconFile.CopyToAsync(_FileStream_);
                    _FileStream_.Close();

                }

                xAdCustomer.StoreIconMD5 = _md5_;
                xAdCustomer.StoreIconContentType = xAdCustomer.StoreIconFile.ContentType;
                xAdCustomer.UserId = UserId_;
                #endregion

                _context.Add(xAdCustomer);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(xAdCustomer);
        }

        #region 

        // GET: XAd/XAdCustomers/Edit/5
        #endregion
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var xAdCustomer = await _context.XAdCustomer.FirstOrDefaultAsync(p=>p.UserId==UserId_ && p.Id==id);
            if (xAdCustomer == null)
            {
                return NotFound();
            }
            ViewData["IsEdit"]=true;
            ViewData[nameof(_context.XAdCustomerLicenses)]=await _context.XAdCustomerLicenses.Where(p=>p.XAdCustomerUserId==UserId_ ).ToListAsync();
            return View("Create",xAdCustomer);
        }

        #region 

        // POST: XAd/XAdCustomers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,LicenseImgFiles,Address,StoreIconFile,Intro,WillBeDeletedLicenseImgIds")] XAdCustomer xAdCustomer)
        {
            if (id != xAdCustomer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(xAdCustomer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!XAdCustomerExists(xAdCustomer.Id))
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
            return View(xAdCustomer);
        }

        #region 

        // GET: XAd/XAdCustomers/Delete/5
        #endregion
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var xAdCustomer = await _context.XAdCustomer
                .FirstOrDefaultAsync(m => m.Id == id);
            if (xAdCustomer == null)
            {
                return NotFound();
            }

            return View(xAdCustomer);
        }


        #region 

        // POST: XAd/XAdCustomers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        #endregion
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var xAdCustomer = await _context.XAdCustomer.FindAsync(id);
            _context.XAdCustomer.Remove(xAdCustomer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool XAdCustomerExists(Guid id)
        {
            return _context.XAdCustomer.Any(e => e.Id == id);
        }

        #region DEPOLLUTION
        #region 
        [HttpGet]
        [AllowAnonymous]
        #endregion
        public async Task<IActionResult> GetXAdCustomerFilesAsync(string MD5 = "")
        {
            if (string.IsNullOrWhiteSpace(MD5))
            {
                return NotFound();
            }

            var _XAdCustomer = await _context.XAdCustomer.FirstOrDefaultAsync(p => p.StoreIconMD5 == MD5);
            var _ContentType = _XAdCustomer?.StoreIconContentType ?? string.Empty;
            var UserId_ = _userManager.GetUserAsync(User)?.GetAwaiter().GetResult()?.Id;
            if (_XAdCustomer == null)
            {
                if (string.IsNullOrWhiteSpace(UserId_)) { return NotFound(); }
                var _XAdCustomerLicenses = await _context.XAdCustomerLicenses.FirstOrDefaultAsync(P => P.XAdCustomerUserId == UserId_ && P.ImgFileMD5 == MD5);
                if (_XAdCustomerLicenses == null)
                {
                    return NotFound();
                }
                _ContentType = _XAdCustomerLicenses.ImgFileContentType;
            }

            var _FilePath = XAdCustomerHelper.XAdCustomerLicenseImgFilePath(_hostingEnv) + $"/{MD5}";

            if (!System.IO.File.Exists(_FilePath))
            {
                return NotFound();
            }


            var _FileStream = new FileStream(_FilePath, FileMode.Open, FileAccess.Read);
            var FileBytes = new byte[(int)_FileStream.Length];
            _FileStream.Read(FileBytes, 0, FileBytes.Length);
            _FileStream.Close();
            return File(FileBytes, _ContentType);
        }

        #endregion
    }
}
