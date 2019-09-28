using System;
using System.Collections.Generic;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;
using POYA.Areas.FunFiles.Models;
using POYA.Data;
using POYA.Unities.Helpers;

namespace POYA.Areas.FunFiles.Controllers
{
    [Area("FunFiles")]
    [Authorize]
    public class FunYourFilesController : Controller
    {
        
        #region DI
        private readonly IWebHostEnvironment _webHostEnv;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly FunFilesHelper _funFilesHelper;
        public FunYourFilesController(
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment webHostEnv,
            IStringLocalizer<Program> localizer)
        {
            _configuration = configuration;
            _webHostEnv = webHostEnv;
            _localizer = localizer;
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _x_DOVEHelper = x_DOVEHelper;
            _signInManager = signInManager;
            _funFilesHelper=new  FunFilesHelper();
        }

        #endregion


        // GET: FunFiles/FunYourFiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.FunYourFile.ToListAsync());
        }

        // GET: FunFiles/FunYourFiles/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var funYourFile = await _context.FunYourFile
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId==UserId_);

            if (funYourFile == null)
            {
                return NotFound();
            }

            var _FunDirs= await _context.FunDir.Where(p=>p.UserId==UserId_).ToListAsync();

            funYourFile.FilePathFunDirs=
                _funFilesHelper.GetPathFunDir(funYourFile.ParentDirId,UserId_,_FunDirs).ToList();

            return View(funYourFile);
        }

        // GET: FunFiles/FunYourFiles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FunFiles/FunYourFiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileByteId,ParentDirId,UserId,Name,DOUploading")] FunYourFile funYourFile)
        {
            if (ModelState.IsValid)
            {
                funYourFile.Id = Guid.NewGuid();
                _context.Add(funYourFile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(funYourFile);
        }

        // GET: FunFiles/FunYourFiles/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var _FunYourFile=await _context.FunYourFile.FirstOrDefaultAsync(p=>p.UserId==UserId_ && p.Id==id);

            if (_FunYourFile == null)
            {
                return NotFound();
            }
            return View(_FunYourFile);
        }

        // POST: FunFiles/FunYourFiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        /// <summary>
        /// Rename only
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,ParentDirId")] FunYourFile funYourFile)
        {
            if (id != funYourFile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
                    var _FunYourFile=await _context.FunYourFile.FirstOrDefaultAsync(p=>p.UserId==UserId_ && p.Id==id);

                    if(_FunYourFile==null)
                    {
                        return NotFound();
                    }
                    
                    _FunYourFile.Name=funYourFile.Name;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunYourFileExists(funYourFile.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index),"FunDirs",new{funYourFile.ParentDirId});
            }
            return View(funYourFile);
        }

        // GET: FunFiles/FunYourFiles/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            
            var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var funYourFile = await _context.FunYourFile.Where(m => m.Id == id && m.UserId==UserId_).FirstOrDefaultAsync();

            if (funYourFile == null)
            {
                return NotFound();
            }

            return View(funYourFile);
        }

        // POST: FunFiles/FunYourFiles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id,bool? IsReturnStatusCodeOnly=true)
        {
            
            var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            
            var funYourFile = await _context.FunYourFile.Where(m => m.Id == id && m.UserId==UserId_).FirstOrDefaultAsync();

            _context.FunYourFile.Remove(funYourFile);
            await _context.SaveChangesAsync();
            if(IsReturnStatusCodeOnly==false)
            {
                return RedirectToAction(nameof(Index),"FunDirs");
            }
            return Content(id.ToString());
        }

        private bool FunYourFileExists(Guid id)
        {
            return _context.FunYourFile.Any(e => e.Id == id);
        }

        #region DEPOLLUTION

        [ActionName("IsLengthOfUserFilesExceeded")]
        public async Task<IActionResult> IsLengthOfUserFilesExceededAsync(long funUploadFileLengthSum) => Content(
                (
                    await IsLengthOfUserFilesExceeded(
                        funUploadFileLengthSum,
                        _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id
                )).ToString());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompareSHA256(SHA256Compare sHA256Compare)
        {
            var _FunFileByteFileSHA256s=await _context.FunFileByte.Select(p=>p.FileSHA256HexString).ToListAsync();

            var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            
            var _FunYourFiles=new List<FunYourFile>();

            sHA256Compare.FileSHA256s
                .Select(
                    p=>new{
                        p.Name,
                        SHA256=p.SHA256HexString})
                .Where(
                    p=>_FunFileByteFileSHA256s.Contains(p.SHA256))
                .ToList()
                .ForEach(
                    p=>{
                        _FunYourFiles.Add(
                            new FunYourFile{
                                DOUploading=DateTimeOffset.Now,
                                Id=Guid.NewGuid(),
                                UserId=UserId_,
                                Name=p.Name,
                                ParentDirId=sHA256Compare.ParentDirId,

                                FileByteId=_context.FunFileByte
                                    .Where(o=>o.FileSHA256HexString==p.SHA256)
                                    .Select(m=>m.Id)
                                    .FirstOrDefaultAsync()
                                    .GetAwaiter()
                                    .GetResult(),
                                    
                        });
                });
            if(_FunYourFiles.Count>0)
            { 
                await _context.FunYourFile.AddRangeAsync(_FunYourFiles);
                await _context.SaveChangesAsync();   
            }

            return Json( 
                sHA256Compare.FileSHA256s.Where(
                    p=>
                        _FunFileByteFileSHA256s.Contains(p.SHA256HexString)
                ).Select(p=>p.Id).ToArray()
            );
        }

        [HttpGet]
        public IActionResult FunUploadFiles(Guid? ParentDirId)
        {
            var _ParentDirId=ParentDirId??_funFilesHelper.RootDirId;
            ViewData["ParentDirId"]=_ParentDirId;
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(1024*1024*1024)]
        public async Task<IActionResult> FunUploadFiles([FromForm]FunUploadFile funUploadFile)
        {


            if (funUploadFile.FunFiles.Count() < 1) return NoContent();

            var _InvalidPathChars = System.IO.Path.GetInvalidPathChars();

            if (
                funUploadFile.FunFiles
                    .Select(p => p.FileName)
                    .Any(p =>
                        _InvalidPathChars
                        .Any(i => p.Contains(i)))
            )
            {
                return NoContent();
            }
            var UserId_ = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            if (await IsLengthOfUserFilesExceeded(
                funUploadFile.FunFiles.Select(p=>p.Length).Sum(), 
                UserId_))
            {
                return BadRequest();
            }


            var _FunFileBytes = new List<FunFileByte>();
            var _FunYourFiles = new List<FunYourFile>();

            funUploadFile.FunFiles.ForEach(p =>
            {

                var _FileBytes = _funFilesHelper.GetFormFileBytes(p);

                var _SHA256 = SHA256.Create().ComputeHash(_FileBytes);
                var _SHA256HexString = _funFilesHelper.SHA256BytesToHexString(_SHA256);

                System.IO.File.WriteAllBytesAsync(
                        _funFilesHelper.FunFilesRootPath(_webHostEnv) + "/" + _SHA256HexString, _FileBytes)
                    .GetAwaiter()
                    .GetResult();

                var _FunFileByte = new FunFileByte
                {
                    DOUploading = DateTimeOffset.Now,
                    FileSHA256HexString = _SHA256HexString,
                    FirstUploaderId = UserId_,
                    Id = Guid.NewGuid()
                };

                _FunFileBytes.Add(_FunFileByte);

                _FunYourFiles.Add(

                    new FunYourFile
                    {

                        DOUploading = DateTimeOffset.Now,
                        Id = Guid.NewGuid(),
                        FileByteId = _FunFileByte.Id,
                        Name = System.IO.Path.GetFileName(p.FileName),
                        ParentDirId = funUploadFile.ParentDirId,
                        UserId = UserId_

                    }
                );
            });

            await _context.FunFileByte.AddRangeAsync(_FunFileBytes);
            await _context.FunYourFile.AddRangeAsync(_FunYourFiles);
            await _context.SaveChangesAsync();
            
            return Ok();
        }

        private async Task<bool>  IsLengthOfUserFilesExceeded(long funUploadFileLengthSum, string UserId_)
        {
            var _FileByteIds = await _context.FunYourFile.Where(p => p.UserId == UserId_).Select(p => p.FileByteId).ToListAsync();
            var _FileSHA256HexStrings = await _context.FunFileByte.Where(p => _FileByteIds.Contains(p.Id)).Select(p => p.FileSHA256HexString).ToListAsync();
            var _AllowedLength=Convert.ToDouble(_configuration["FunYourFiles:AllowedLengthForPerUser"]);

            var _FunYourFileSizeSum = new DirectoryInfo(_funFilesHelper.FunFilesRootPath(_webHostEnv)).GetFiles()
                .Where(p => _FileSHA256HexStrings.Contains(p.Name)).Select(p => p.Length).Sum();

            _FunYourFileSizeSum = _FunYourFileSizeSum + funUploadFileLengthSum;

            return _FunYourFileSizeSum > _AllowedLength;
        }

        [ActionName("DownloadFunFile")]
        public async Task<IActionResult> DownloadFunFileAsync(Guid? id)
        {
            if(id==null)
            {
                return NotFound();
            }
            var User_= _userManager.GetUserAsync(User).GetAwaiter().GetResult();

            var _FunYourFile=await _context.FunYourFile.Where(p=>p.Id==id && p.UserId==User_.Id).Select(p=>new{p.FileByteId,p.Name}).FirstOrDefaultAsync();

            var _FunFileByte=await _context.FunFileByte.Where(p=>p.Id==_FunYourFile.FileByteId).Select(p=>new{p.FileSHA256HexString}).FirstOrDefaultAsync();
            
            var _FileBytes=await System.IO.File.ReadAllBytesAsync(
                _funFilesHelper.FunFilesRootPath(_webHostEnv)+"/"+_FunFileByte.FileSHA256HexString
            );

            return File(_FileBytes,_funFilesHelper.GetContentType(_FunYourFile.Name),_FunYourFile.Name,true);
        }

        #endregion
    }
}
