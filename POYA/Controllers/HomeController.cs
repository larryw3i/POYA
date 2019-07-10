﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using POYA.Areas.XAd.Controllers;
using POYA.Data;
using POYA.Models;
using POYA.Unities.Attributes;
using POYA.Unities.Helpers;
namespace POYA.Controllers
{
    public class HomeController : Controller
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
        private readonly IConfiguration _configuration;
        public HomeController(
            IConfiguration configuration,
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
            _configuration = configuration;
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
        public IActionResult Index()
        {
            //  throw new Exception("TEST"); 

            return View();
        }

        //  [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Donate()
        {
            return View();
        }


        #region DEPOLLUTION

        [ActionName("GetLAppContent")]
        public async Task<IActionResult> GetLAppContentAsync(string ContentNmae)
        {
            #region REFUSE
            if (ContentNmae.Contains("..") ||
                ContentNmae.StartsWith('/') ||
                ContentNmae.StartsWith('\\') ||
                ContentNmae.Contains('~') ||
                ContentNmae.Contains("\\\\") ||
                ContentNmae.Contains("//") ||
                ContentNmae.Contains("'") ||
                ContentNmae.Contains("\"")) return NoContent();
            #endregion

            var provider = new FileExtensionContentTypeProvider();
            string contentType = string.Empty;
            var _LAppContentPath = _hostingEnv.ContentRootPath + "/Data/LAppContent/";
            var _FileBytes = await System.IO.File.ReadAllBytesAsync($"{_LAppContentPath}/{ContentNmae}");

            if (!provider.TryGetContentType(ContentNmae, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return File(_FileBytes, contentType);
        }

        public async Task<IActionResult> GetAvatar(string UserId = "")
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserId = string.IsNullOrWhiteSpace(UserId) ? CurrentUserId : UserId;
            var AvatarFilePath = X_DOVEValues.AvatarStoragePath(_hostingEnv) + UserId;
            //  var _X_doveUserInfo=await _context.X_DoveUserInfos.Where(p => p.UserId == UserId).FirstOrDefaultAsync();
            if (System.IO.File.Exists(AvatarFilePath))
            {
                var AvatarBytes = await System.IO.File.ReadAllBytesAsync(AvatarFilePath);
                if (AvatarBytes != null || AvatarBytes.Length > 1)
                {
                    return File(AvatarBytes, "image/jpg");
                }
            }
            var DefauleAvatar = await System.IO.File.ReadAllBytesAsync(_hostingEnv.ContentRootPath + @"/Data/LAppContent/img/article_publish_ico.webp");
            return File(DefauleAvatar, "image/webp");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> UploadAvatar([FromForm]IFormFile avatarFile)
        {
            if (avatarFile.Length > 1024 * 1024)
            {
                return Json(new { status = false, msg = "ExceedSize" });   //  _localizer["Oops...., the size of avatar should be less than 1M"] 
            }

            var _allowedAvatarFileExtensions = new string[] { "image/jpg", "image/jpeg", "image/png" };
            if (!_allowedAvatarFileExtensions.Contains(avatarFile.ContentType.ToLower()))
            {
                return Json(new { status = false, msg = "RefuseExtension" });
            }
            var _UserId = _userManager.GetUserAsync(User).GetAwaiter().GetResult().Id;
            var memoryStream = new MemoryStream();
            await avatarFile.CopyToAsync(memoryStream);
            var AvatarBytes = MakeCircleImage(memoryStream);//   memoryStream.ToArray();   //  
            var AvatarFilePath = X_DOVEValues.AvatarStoragePath(_hostingEnv) + _UserId;
            if (System.IO.File.Exists(AvatarFilePath))
            {
                var AvatarFileBytes = await System.IO.File.ReadAllBytesAsync(AvatarFilePath);
                var _X_doveUserInfo = await _context.X_DoveUserInfos.FirstOrDefaultAsync(p => p.UserId == _UserId);
                if (_X_doveUserInfo != null && AvatarBytes == AvatarFileBytes)
                {//  _X_doveUserInfo.AvatarBuffer
                    return Json(new { status = true });  //  , X_DOVE_XSRF_TOKEN 
                }
            }
            await System.IO.File.WriteAllBytesAsync(X_DOVEValues.AvatarStoragePath(_hostingEnv) + _UserId, AvatarBytes);
            //  X_DOVE_XSRF_TOKEN = Guid.NewGuid().ToString();
            //  TempData["X_DOVE_XSRF_TOKEN"] = X_DOVE_XSRF_TOKEN;
            return Json(new { status = true }); //  , X_DOVE_XSRF_TOKEN
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

            #region COMPRESS
            /**
             * We had to make some sacrifices in order to the load faster
             * REFERENCE    https://docs.microsoft.com/en-us/dotnet/framework/winforms/advanced/how-to-set-jpeg-compression-level
             * THANK        https://github.com/dotnet/docs/blob/master/docs/framework/winforms/advanced/how-to-set-jpeg-compression-level.md
             */
            var AvatarEncoderParameters = new EncoderParameters(1);
            AvatarEncoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 8L);
            #endregion

            b.Save(ms, ImageCodecInfo.GetImageDecoders().FirstOrDefault(p => p.FormatID == ImageFormat.Jpeg.Guid), AvatarEncoderParameters);
            return ms.ToArray();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult KeepLogin()
        {
            return Ok();
        }

        /*
        private async Task  AppInitializationAsync()
        {

            var LFilesPath = _hostingEnv.ContentRootPath + "/Data/LFiles";
            var AvatarPath = $"{LFilesPath}/Avatars";
            var XUserFilePath = $"{LFilesPath}/XUserFile";
            var EArticleFilesPath = $"{_hostingEnv.ContentRootPath}/Areas/EduHub/Data/EArticleFiles";

            var InitialPaths = new string[] { AvatarPath, XUserFilePath, XAdCustomerHelper.XAdCustomerLicenseImgFilePath(_hostingEnv), EArticleFilesPath };
            foreach (var p in InitialPaths)
            {
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
            }
            #region INITIAL_USER_ROLE
            var _userRoles = new string[] { X_DOVEValues._administrator };
            var _userRoles_ =await _context.Roles.Select(p => p.Name).ToListAsync();    //  .GetAwaiter().GetResult();
            _userRoles = _userRoles.Where(p => !_userRoles_.Contains(p)).ToArray();
            if (_userRoles.Count() > 0)
            {
                foreach (var r in _userRoles)
                {
                    await _context.Roles.AddAsync(new IdentityRole { Name = r });   //  .GetAwaiter().GetResult();
                    //  _roleManager.CreateAsync(
                }
            }
            
            await _context.SaveChangesAsync();

            var _user =await _context.Users.FirstOrDefaultAsync(p => p.Email == _configuration["Administration:AdminEmail"]);   //  .GetAwaiter().GetResult();    
            //  FindByEmailAsync(configuration["Administration:AdminEmail"]).GetAwaiter().GetResult();
            if (_user != null)
            {
#if DEBUG
                Console.WriteLine($"XMsg --->\tAdministrator is exist");
                var _roleId =_context.Roles.FirstOrDefaultAsync(p=>p.Name==X_DOVEValues._administrator).GetAwaiter().GetResult().Id;
#endif  
                if (_roleId!=null && !_context.UserRoles.AnyAsync(p=>p.UserId==_user.Id && p.RoleId==_roleId).GetAwaiter().GetResult())   //  _userManager.IsInRoleAsync(_user, _administrator).GetAwaiter().GetResult()
                {
#if DEBUG
                    Console.WriteLine($"XMsg --->\tAdd role to administrator . . .");
#endif
                    await _context.UserRoles.AddAsync(new IdentityUserRole<string> {  RoleId=_roleId, UserId=_user.Id});
                    //  _userManager.AddToRoleAsync(_user, _administrator).GetAwaiter().GetResult();
#if DEBUG
                    Console.WriteLine($"XMsg --->\tAdd role to administrator is finish");
#endif
                }
            }
            await _context.SaveChangesAsync();

            #endregion

            #region MODIFY appsettings.json
            var _appsettingsPath=_hostingEnv.ContentRootPath+$"/appsettings.json";
            string _output =await System.IO.File.ReadAllTextAsync(_appsettingsPath);
            dynamic _appsettings=JsonConvert.DeserializeObject(_output);
            _appsettings["IsInitialized"]=true;
            _output = Newtonsoft.Json.JsonConvert.SerializeObject(_appsettings, Newtonsoft.Json.Formatting.Indented);
            await System.IO.File.WriteAllTextAsync(_appsettingsPath,_output);
            #endregion

        }
        */

        #endregion

    }
}
