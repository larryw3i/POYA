

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using POYA.Areas.WeEduHub.Models;

namespace POYA.Areas.WeEduHub.Controllers
{
    public class WeEduHubHelper
    {
        /// <summary>
        /// _hostingEnv.ContentRootPath+"/Areas/WeEduHub/Data/WeEduHubFiles"
        /// </summary>
        /// <param name="_hostingEnv"></param>
        /// <returns></returns>
        public  string  WeEduHubFilesDirectoryPath(IHostingEnvironment _hostingEnv)=>_hostingEnv.ContentRootPath+"/Areas/WeEduHub/Data/WeEduHubFiles";

    }

}