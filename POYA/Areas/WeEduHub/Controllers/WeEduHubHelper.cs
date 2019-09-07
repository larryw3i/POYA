

using Microsoft.AspNetCore.Hosting;

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