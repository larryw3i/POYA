using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XAd.Controllers
{
    public static class XAdCustomerHelper
    {
        #region 
        /// <summary>
        /// _hostingEnv.ContentRootPath + $"/Areas/XAd/Data/LicenseImgFiles"
        /// </summary>
        /// <param name="_hostingEnv"></param>
        /// <returns></returns>
        #endregion
        public static string XAdCustomerLicenseImgFilePath(IHostingEnvironment _hostingEnv) => _hostingEnv.ContentRootPath + $"/Areas/XAd/Data/LicenseImgFiles";
    }
}
