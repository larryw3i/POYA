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
        /// (IHostingEnvironment _hostingEnv) => _hostingEnv.ContentRootPath + $"/Areas/XAd/Data/XAdImgFiles"
        /// </summary>
        /// <param name="_hostingEnv"></param>
        /// <returns></returns>
        #endregion
        public static string XAdImgFilePath(IHostingEnvironment _hostingEnv) => _hostingEnv.ContentRootPath + $"/Areas/XAd/Data/XAdImgFiles";
    }
}
