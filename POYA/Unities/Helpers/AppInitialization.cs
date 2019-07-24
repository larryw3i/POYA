using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using POYA.Areas.XAd.Controllers;
using POYA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Unities.Helpers
{
    /// <summary>
    /// Application initialization
    /// </summary>
    public class AppInitialization
    {
        public string _administrator = "Administrator";
        public AppInitialization(IServiceCollection services)
        {
            var _buildServiceProvider = services.BuildServiceProvider();
            var _hostingEnv = _buildServiceProvider.GetService<IHostingEnvironment>();
            var _context = _buildServiceProvider.GetService<ApplicationDbContext>();
            var _configuration = _buildServiceProvider.GetRequiredService<IConfiguration>();
            var LFilesPath = _hostingEnv.ContentRootPath + "/Data/LFiles";
            var AvatarPath = $"{LFilesPath}/Avatars";
            var XUserFilePath = $"{LFilesPath}/XUserFile";
            var EArticleFilesPath = $"{_hostingEnv.ContentRootPath}/Areas/EduHub/Data/EArticleFiles";

            var InitialPaths = new string[] { AvatarPath, XUserFilePath, XAdCustomerHelper.XAdImgFilePath(_hostingEnv), EArticleFilesPath };
            foreach (var p in InitialPaths)
            {
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
            }
         
            #region DEPOLLUTION

         
            #endregion

        }
    }
}
