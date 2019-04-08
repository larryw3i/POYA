using System;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using POYA.Data;
using POYA.Models;
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
        public HomeController(
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
        public IActionResult Test()
        {
            return View();
        }
        public async Task<IActionResult> GetAvatar(string UserId="")
        {
            var CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserId =  string.IsNullOrWhiteSpace(UserId) ? CurrentUserId : UserId;
            var _X_doveUserInfo=await _context.X_DoveUserInfos.Where(p => p.UserId == UserId).FirstOrDefaultAsync();
            if (_X_doveUserInfo == null || _X_doveUserInfo.AvatarBuffer.Length<1)
            {
                var DefauleAvatar = await System.IO.File.ReadAllBytesAsync(_hostingEnv + @"\img\article_publish_ico.png");
                return File(DefauleAvatar, "image/png");
            }
            return File(_X_doveUserInfo.AvatarBuffer, "image/png");
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
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
