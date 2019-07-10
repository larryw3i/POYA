using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nukepayload2.Csv;
using POYA.Areas.XUserFile.Models;
using POYA.Data;
using POYA.Models;
using POYA.Unities.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
namespace POYA.Unities.Helpers
{
    public class X_DOVEHelper
    {

        #region SOME VALUES



        /// <summary>
        /// Get the full path of the dir file or directory is included
        /// </summary>
        /// <param name="id">The id of file or directory</param>
        /// <param name="context">The context</param>
        /// <param name="InDirId">The id of directory contain file or directory</param>
        /// <returns></returns>
        public string GetInPathOfFileOrDir(ApplicationDbContext context, Guid InDirId)
        {
            var FullPath = string.Empty;
            for (var i = 0; i < 30 && InDirId != Guid.Empty; i++)
            {
                var InDir = context.LDir.Where(p => p.Id == InDirId).Select(p => new { p.InDirId, p.Name }).FirstOrDefaultAsync()
                    .GetAwaiter().GetResult();
                if (InDir?.InDirId == null) break;
                FullPath = $"{InDir.Name}/{FullPath}";
                InDirId = InDir.InDirId;
            }
            return $"root/{FullPath}";
        }

        #endregion

        #region SOME METHODS


        /// <summary>
        /// Send error log to email
        /// REFERENCE   https://blog.csdn.net/confused_kitten/article/details/81702861
        /// THANK       https://blog.csdn.net/confused_kitten
        /// </summary>
        /// <param name="context"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task ErrorEventAsync(
            IConfiguration configuration,
            HttpContext context
            )
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            var MarkLineReg = new Regex("(:line\\s?\\d+)");
            var _stackTrace = error.StackTrace;
            var MarkLineMatchs = MarkLineReg.Matches(_stackTrace);
            foreach (var i in MarkLineMatchs)
            {
                _stackTrace = _stackTrace.Replace(oldValue: i.ToString(), newValue: $"<span style='color:red'><strong>{i}</strong></span>");
            }
            await new EmailSender(configuration).SendEmailAsync(
                email: configuration["ErrorLogHandle:ReceiveLogEmailAddress"],//   (string)jsonObject[nameof(X_DOVEHelper)]["email"],
                subject: "X_DOVE_ERROR",
                htmlMessage:
            #region     HTMLMESSAGE
                $@"
                    <table border='1'>
                        <tr>
                            <td style='min-width:10%'>
                                ERROR
                            </td>
                            <td>
                                DETAILS
                            </td>
                        </tr>
                        <tr>
                            <td style='min-width:10%'> Message
                            </td>
                            <td> {error.Message  }
                            </td>
                        </tr> 
                        <tr>
                            <td style='min-width:10%'> Data
                            </td>
                            <td> {error.Data }
                            </td>
                        </tr>  
                        <tr>
                            <td style='min-width:10%'> InnerException
                            </td>
                            <td> {error.InnerException }
                            </td>
                        </tr>  
                        <tr>
                            <td style='min-width:10%'> StackTrace
                            </td>
                            <td> { _stackTrace }
                            </td>
                        </tr>
                    </table>"
            #endregion
                );
        }
        #endregion
    }

    #region 
    /*
   public class MimeHelper
   {

       public List<string> GetExtensions(string Mime, IHostingEnvironment env)
       {
           var _CSV = File.ReadAllTextAsync(env.ContentRootPath + "/Data/LAppContent/lmime.csv").GetAwaiter().GetResult();
           var MediaTypes = CsvConvert.DeserializeObject<MediaType>(_CSV);
           var MediaTypeList = MediaTypes.ToList();    //  .Select(p=>new MediaType {  Name=});
           var _Extensions = new List<string>();
           Mime = Mime.Split(".").LastOrDefault().ToLower();
           foreach (var i in MediaTypeList)
           {
               if (i.Template.ToLower() == Mime)
               {
                   _Extensions.Add(i.Name);
               }
           }
           return _Extensions;
       }
   }
   */
    #endregion


    public static class X_DOVEValues
    {
        /// <summary>
        /// The Guid of public directory
        /// </summary>
        public static Guid PublicDirId{get;} = Guid.Parse("E1F500C2-FCF4-4BD8-B54A-A2A7A41F793C");

        /// <summary>
        /// env.ContentRootPath + $"/Data/LFiles/Avatars/"
        /// </summary>
        /// <param name="env">
        /// The HostingEnvironment
        /// </param>
        /// <returns></returns>
        public static string AvatarStoragePath(IHostingEnvironment env) => env.ContentRootPath + $"/Data/LFiles/Avatars/";


        /// <summary>
        /// env.ContentRootPath+$"/Data/LFiles/EduHub/"
        /// </summary>
        /// <param name="env">The HostingEnvironment</param>
        /// <returns>env.ContentRootPath+$"/Data/LFiles/EduHub/"</returns>
        public static string EduHubFileStoragePath(IHostingEnvironment env) => env.ContentRootPath + $"/Data/LFiles/EduHub/";

        /// <summary>
        /// env.ContentRootPath + $"/Data/LFiles/XUserFile/"
        /// </summary>
        /// <param name="env">The HostingEnvironment</param>
        /// <returns></returns>
        public static string FileStoragePath(IHostingEnvironment env) => env.ContentRootPath + $"/Data/LFiles/XUserFile/";

        /// <summary>
        /// The default earticle set id
        /// </summary>
        /// <returns></returns>
        public static Guid DefaultEArticleSetId { get; } = Guid.Parse("5F8EBDF5-1ADC-4891-85F6-FD0755223A06");

        /// <summary>
        /// The add-earticle-set id
        /// </summary>
        /// <returns></returns>
        public static Guid AddEArticleSetId { get; } = Guid.Parse("F834D7E8-BEB8-42F4-A724-25099054863D");
        
        /// <summary>
        /// The max multipart body length limit 
        /// </summary>
        public static long MaxMultipartBodyLengthLimit = 200_000_000;

        /// <summary>
        /// "Administrator"
        /// </summary>
        public static string _administrator = "Administrator";

    }




    #region DISCARD
    /*

        /// <summary>
        /// Indicate whether the applicaton is initialized or not
        /// </summary>
        /// <value></value>
        public static bool IsInitialized{get;set;}=false;

    public static class LValue
    {
        public static Guid PublicDirId { get; } = Guid.Parse("75EAD9A8-31C0-4491-8D8B-431A506C6567");
        public static Guid DefaultEArticleSetId { get; } = Guid.Parse("5F8EBDF5-1ADC-4891-85F6-FD0755223A06");
        public static Guid AddEArticleSetId { get; } = Guid.Parse("F834D7E8-BEB8-42F4-A724-25099054863D");
        
        public static long MaxMultipartBodyLengthLimit = 200_000_000;

    }


    /// <summary>
    /// FROM    https://www.cnblogs.com/xishuai/p/asp-net-core-ioc-di-get-service.html
    /// THANK   https://www.cnblogs.com/xishuai/
    /// </summary>
    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }
    }


    public List<string> GetMimes(string FileExtension, IHostingEnvironment env)
    {
        var _SlnPath = $"/Users/larry/source/repos/POYA";
        var _MimeDirPath = $"{_SlnPath}/POYA/Data/LAppDoc";
        var _mimeJson = File.ReadAllTextAsync(_MimeDirPath + "/lmime.json").GetAwaiter().GetResult();
        var _mimes = (JArray)JsonConvert.DeserializeObject(_mimeJson);
        var _mimes_ = new List<string>();
        FileExtension = FileExtension.Contains(".") ? FileExtension.Split(".").LastOrDefault() : FileExtension;
        foreach (var i in _mimes)
        {
            var _milk = i.ToString().Split('\n')[1];
            var _extension = _milk.Split('\"')[1];
            var _mime = _milk.Split("\"")[3];
            if (FileExtension == _extension.ToLower()) _mimes_.Add(_mime);
        }
        if (_mimes_.Count() < 1) _mimes_.Add("text/plain");
        return _mimes_;
        #region OLD_VERSION
        *
        var _CSV = File.ReadAllTextAsync(env.ContentRootPath + "/Data/LAppDoc/lmime.csv").GetAwaiter().GetResult();
        var MediaTypes = CsvConvert.DeserializeObject<MediaType>(_CSV);
        FileExtension = FileExtension.Contains(".") ? FileExtension.Split(".").LastOrDefault() : FileExtension;
        //  Console.WriteLine(JsonConvert.SerializeObject(MediaTypes));
        var MediaTypeList = MediaTypes.ToList();
        var _MediaTypes = MediaTypeList.Where(p => p.Name == FileExtension).Select(p => p.Template).ToList(); // "text/plain";
        //  _MediaTypes = _MediaTypes.Count() < 1 ? "text/plain" : _MediaTypes;
        if (_MediaTypes.Count() < 1)
        {
            _MediaTypes.Add("text/plain");
        }
        return _MediaTypes;
        *
        #endregion

    }*/
    #endregion
}
