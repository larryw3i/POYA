

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
        public string WEARTICLE_SORT_BY_String="WEARTICLE_SORT_BY";
        public string IS_WEARTICLE_ORDER_BY_ASC_String="IS_WEARTICLE_ORDER_BY_ASC";
        public string SortByDate="b5210ed8-2803-43bf-bce5-f686887cd4e1";
        public string SortByTitle="3ddb379e-03c7-47e7-9b4a-34b55a35822e";
        public string SortByModifying="83e2c8be-7d52-481d-8d20-9131b3c96c8f";

        /// <summary>
        /// _hostingEnv.ContentRootPath+"/Areas/WeEduHub/Data/WeEduHubFiles"
        /// </summary>
        /// <param name="_hostingEnv"></param>
        /// <returns></returns>
        public  string  WeEduHubFilesDirectoryPath(IHostingEnvironment _hostingEnv)=>_hostingEnv.ContentRootPath+"/Areas/WeEduHub/Data/WeEduHubFiles";

    }

}