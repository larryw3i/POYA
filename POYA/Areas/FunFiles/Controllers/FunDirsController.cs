using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
    [Authorize]
    [Area("FunFiles")]    
    public class FunDirsController : Controller
    {
       
        #region DI
        private readonly IHostingEnvironment _hostingEnv;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly X_DOVEHelper _x_DOVEHelper;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IStringLocalizer<Program> _localizer;
        private readonly FunFilesHelper _funFilesHelper;
        public FunDirsController(
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            X_DOVEHelper x_DOVEHelper,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context,
            IHostingEnvironment hostingEnv,
            IStringLocalizer<Program> localizer)
        {
            _configuration = configuration;
            _hostingEnv = hostingEnv;
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

        // GET: FunFiles/FunDirs
        public async Task<IActionResult> Index(Guid? ParentDirId)
        {
            var User_=await _userManager.GetUserAsync(User);
            var _ParentDirId=ParentDirId??_funFilesHelper.RootDirId;
            var _FunDir=await _context.FunDir
                .Where(p=>p.UserId==User_.Id && p.ParentDirId==_ParentDirId)
                .ToListAsync();
            
            var _FunYourFiles=await _context.FunYourFile
                .Where(p=>p.UserId==User_.Id && p.ParentDirId==_ParentDirId)
                .ToListAsync();

            _FunYourFiles.ForEach(
                o=>{
                    o.FileSize=
                        _funFilesHelper.OptimizeFileSize(
                            new System.IO.FileInfo(
                                    _funFilesHelper.FunFilesRootPath(_hostingEnv)+'/'+
                                        _context.FunFileByte.Where(f=>f.Id==o.FileByteId).Select(p=>p.FileSHA256HexString)
                                            .FirstOrDefaultAsync()
                                            .GetAwaiter()
                                            .GetResult()
                            )
                            .Length
                        );
                }
            );


            ViewData[nameof(FunYourFile)+"s"]= _FunYourFiles;

            ViewData[nameof(ParentDirId)]=_ParentDirId;

            ViewData["FunDirPath"]=await _funFilesHelper.GetFunDirPathAsync(ParentDirId,User_.Id,_context);

            return View(_FunDir);
        }

        // GET: FunFiles/FunDirs/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var User_=await _userManager.GetUserAsync(User);
            
            var funDir = await _context.FunDir
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId==User_.Id);

            if (funDir == null)
            {
                return NotFound();
            }
            

            return View(funDir);
        }

        // GET: FunFiles/FunDirs/Create
        public IActionResult Create(Guid? ParentDirId)
        {
            var _FunDir=new FunDir
            {
                ParentDirId=ParentDirId??_funFilesHelper.RootDirId
            };
            return View(_FunDir);
        }

        // POST: FunFiles/FunDirs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ParentDirId,Name")] FunDir funDir)
        {
            if (ModelState.IsValid)
            {
                var _FunDirNameLength=funDir?.Name?.Length??0;

                if(_FunDirNameLength<1||_FunDirNameLength>100){

                    ModelState.AddModelError(
                        nameof(funDir.Name),
                        _localizer["The length of directory name must be less than 100 and more than 1"]);
                        
                    return View();

                }

                var User_=await _userManager.GetUserAsync(User);

                funDir.Id = Guid.NewGuid();
                funDir.DOCreating=DateTimeOffset.Now;
                funDir.UserId=User_.Id;

                _context.Add(funDir);
                await _context.SaveChangesAsync();
                return RedirectToAction(
                    nameof(Index),
                    new{ ParentDirId=funDir.ParentDirId }
                );
            }
            return View(funDir);
        }

        // GET: FunFiles/FunDirs/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var funDir = await _context.FunDir.FindAsync(id);
            if (funDir == null)
            {
                return NotFound();
            }
            return View(funDir);
        }

        // POST: FunFiles/FunDirs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name")] FunDir funDir)
        {
            if (id != funDir.Id)
            {
                return NotFound();
            }

            var ParentDirId=Guid.Empty;

            if (ModelState.IsValid)
            {
                try
                {
                    var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

                    var _FunDir=await _context.FunDir.Where(p=>p.Id==funDir.Id && p.UserId==UserId_).FirstOrDefaultAsync();
                    
                    if(_FunDir!=null)
                    {
                        _FunDir.Name=funDir.Name;
                    } 
                    
                    await _context.SaveChangesAsync();

                    ParentDirId=_FunDir.ParentDirId;
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FunDirExists(funDir.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index),new{ParentDirId});
            }
            return View(funDir);
        }

        // GET: FunFiles/FunDirs/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;

            var funDir = await _context.FunDir
                .Where(m => m.Id == id && m.UserId==UserId_).FirstOrDefaultAsync();

            if (funDir == null)
            {
                return NotFound();
            }
            

            return View(funDir);
        }

        // POST: FunFiles/FunDirs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var UserId_= _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            
            var funDir = await _context.FunDir.Where(m=>m.Id==id && m.UserId==UserId_).FirstOrDefaultAsync();

            _context.FunDir.Remove(funDir);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FunDirExists(Guid id)
        {
            return _context.FunDir.Any(e => e.Id == id);
        }

        #region DEPOLLUTION
        

        [HttpGet]
        [ActionName("CopyDirOrFile")]
        public async Task<IActionResult> CopyDirOrFileAsync(Guid ParentDirId, Guid Id)
        {
            var User_=await _userManager.GetUserAsync(User);

            if(
                ParentDirId!=_funFilesHelper.RootDirId && 
                !await _context.FunDir.Where(p=>p.UserId==User_.Id).AnyAsync(p=>p.Id==ParentDirId)
            )   
            {
                return NotFound();
            }

            var _FunDirs=await _context.FunDir.Where(p=> p.UserId==User_.Id).ToListAsync();

            var _FunYourFiles=await _context.FunYourFile.Where(p=> p.UserId==User_.Id).ToListAsync();

            var TargetFunDir=_FunDirs.Where(p=>p.Id==Id).FirstOrDefault();

            var TargetFunYourFile=_FunYourFiles.Where(p=>p.Id==Id).FirstOrDefault();


            var IdAndParentIds= _FunDirs
                .Select(
                    p=>
                        new IdAndParentId
                        {
                            Id=p.Id,
                            ParentId=p.ParentDirId
                        }
                )
                .Union(
                    _FunYourFiles
                    .Select(
                        p=>
                            new IdAndParentId
                            {
                                Id=p.Id,
                                ParentId=p.ParentDirId
                            }
                    )
                )
                .ToList();


            // Directory
            if(TargetFunDir!=null)
            { 
                var _NewFunYourFiles=new List<FunYourFile>();
                
                var _NewFunDirs=new List<FunDir>();

                var _FunNewIds=new List<FunNewId>()
                    {
                        new FunNewId{
                            Id=TargetFunDir.Id,
                            NewId=Guid.NewGuid()
                        }
                    };

                _FunDirs.Select(p=>p.Id)
                    .Union(
                        _FunYourFiles
                            .Select(p=>p.Id))
                    .ToList()
                    .ForEach(
                        p=>
                            {
                                if(_funFilesHelper.IsIdInParentDirId(TargetFunDir.Id, p, IdAndParentIds))
                                {
                                    _FunNewIds.Add(
                                        new FunNewId
                                        {
                                            Id=p,
                                            NewId=Guid.NewGuid()
                                        }
                                    );
                                }
                            }
                );


                _FunNewIds.ForEach(
                    p=>
                    {
                        var _ParentDirId_=_FunNewIds
                            .Where(
                                n=>
                                    n.Id== IdAndParentIds
                                        .Where(i=>i.Id==p.Id)
                                            .Select(i=>i.ParentId).FirstOrDefault()
                            ).Select(o=>o.NewId).FirstOrDefault();

                        _ParentDirId_=_ParentDirId_==Guid.Empty?ParentDirId:_ParentDirId_;

                        // is file
                        if(_FunYourFiles.Any(f=>f.Id==p.Id))
                        {
                            var _FunYourFile_=_FunYourFiles.Where(f=>f.Id==p.Id).FirstOrDefault();
                            _NewFunYourFiles.Add( 
                                new FunYourFile{
                                    DOUploading=DateTimeOffset.Now,
                                    FileByteId=_FunYourFile_.FileByteId,
                                    Id=p.NewId,
                                    Name=_FunYourFile_.Name,
                                    ParentDirId=_ParentDirId_,
                                    UserId=User_.Id
                                }
                            );
                        }
                        // is directory
                        else
                        {
                            var _FunDir_=_FunDirs.Where(d=>d.Id==p.Id).FirstOrDefault();
                            _NewFunDirs.Add( 
                                new FunDir{
                                    DOCreating=DateTimeOffset.Now,
                                    Id=p.NewId,
                                    Name=_FunDir_.Name,
                                    ParentDirId=_ParentDirId_,
                                    UserId=User_.Id
                                }
                            );

                        }
                    }
                );

                if(_NewFunDirs.Count()>0) await _context.FunDir.AddRangeAsync(_NewFunDirs);
                
                if(_NewFunYourFiles.Count()>0) await _context.FunYourFile.AddRangeAsync(_NewFunYourFiles);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index),new{ ParentDirId });
            }
            // File
            else  if(TargetFunYourFile!=null)
            {
                await _context.FunYourFile.AddAsync(
                    new FunYourFile
                    {
                        Id=Guid.NewGuid(),
                        ParentDirId=ParentDirId,
                        FileByteId=TargetFunYourFile.FileByteId,
                        DOUploading=DateTimeOffset.Now,
                        Name=TargetFunYourFile.Name,
                        UserId=User_.Id,
                        
                    }
                );

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index),new{ ParentDirId });
            }

            return NotFound();
        }

        [HttpGet]
        [ActionName("MoveDirOrFile")]
        public async Task<IActionResult> MoveDirOrFileAsync(Guid ParentDirId, Guid Id)
        {
            var User_=await _userManager.GetUserAsync(User);
            var _FunDir=await _context.FunDir.Where(p=>p.Id==Id && p.UserId==User_.Id).FirstOrDefaultAsync();
            if(_FunDir!=null)
            {
                _FunDir.ParentDirId=ParentDirId; 
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),new{ ParentDirId });
            }
            else
            {
                var _FunYourFile=await _context.FunYourFile.Where(p=>p.Id==Id && p.UserId==User_.Id).FirstOrDefaultAsync();
                if(_FunYourFile!=null)
                {
                    _FunYourFile.ParentDirId=ParentDirId; 
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index),new{ ParentDirId });
                }
            }

            return NotFound();
        }

    
        public async Task<IActionResult> DirIndex(Guid? ParentDirId,Guid? CurrentMovingDirId)
        {
            var User_=await _userManager.GetUserAsync(User);
            var _ParentDirId=ParentDirId??_funFilesHelper.RootDirId;
            var _FunDirs=await _context.FunDir
                .Where(p=>p.UserId==User_.Id && p.ParentDirId==_ParentDirId)
                .ToListAsync();

                if(CurrentMovingDirId!=null)
                {
                    _FunDirs.Remove(_FunDirs.FirstOrDefault(p=>p.Id==CurrentMovingDirId));
                }

            ViewData[nameof(ParentDirId)]=_ParentDirId;

            ViewData["FunDirPath"]=await _funFilesHelper.GetFunDirPathAsync(ParentDirId,User_.Id,_context);

            return View(_FunDirs);
        }


        /// <summary>
        /// GET: FunFiles/FunDirs/iDetails/5, list dir and file in Details Action
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> iDetails(Guid? id)
        {
            id=id??_funFilesHelper.RootDirId;

            var User_=await _userManager.GetUserAsync(User);
            
            var funDir = await _context.FunDir
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId==User_.Id);
                
            if (funDir == null)
            {
                return NotFound();
            }
            
            return View(funDir);
        }

        public async Task<IActionResult> IndexFunYourFiles(Guid? ParentDirId)
        {
            var _ParentDirId=ParentDirId??_funFilesHelper.RootDirId;
            var User_=await _userManager.GetUserAsync(User);
            return View(await _context.FunYourFile
                .Where(p=>p.ParentDirId==_ParentDirId && p.UserId==User_.Id)
                .ToListAsync()
            );
        }

        public async Task<IActionResult> IndexFunYourFilesJson(Guid? ParentDirId)
        {
            var _ParentDirId=ParentDirId??_funFilesHelper.RootDirId;
            var User_=await _userManager.GetUserAsync(User);
            return Json(await _context.FunYourFile
                .Where(p=>p.ParentDirId==_ParentDirId && p.UserId==User_.Id)
                .ToListAsync()
            );
        }

        #endregion
    }
}
