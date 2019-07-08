using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using POYA.Areas.XAd.Controllers;
using POYA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Unities.Helpers
{
    public class AppInitialization
    {
        public AppInitialization(IServiceCollection services, IHostingEnvironment env)
        {
            var _roleManager = services.BuildServiceProvider().GetService<RoleManager<IdentityRole>>();
            var _context = services.BuildServiceProvider().GetService<ApplicationDbContext>();

            #region CUSTOM_APP_INITIALIZATION
            var LFilesPath = env.ContentRootPath + "/Data/LFiles";
            var AvatarPath = $"{LFilesPath}/Avatars";
            var XUserFilePath = $"{LFilesPath}/XUserFile";
            var EArticleFilesPath = $"{env.ContentRootPath}/Areas/EduHub/Data/EArticleFiles";

            var InitialPaths = new string[] { AvatarPath, XUserFilePath, XAdCustomerHelper.XAdCustomerLicenseImgFilePath(env), EArticleFilesPath };
            foreach (var p in InitialPaths)
            {
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
            }
            #endregion

            #region INITIAL_USER_ROLE
            var _userRoles = new string[] { "Administrator" };
            var _userRoles_ = _roleManager.Roles.Select(p => p.Name).ToListAsync().GetAwaiter().GetResult();
            _userRoles = _userRoles.Where(p => !_userRoles_.Contains(p)).ToArray();
            if (_userRoles.Count() > 0) {
                foreach (var r in _userRoles)
                {
                    _roleManager.CreateAsync(new IdentityRole { Name = r }).GetAwaiter().GetResult();
                }
            }
            #endregion
        }
    }
}
