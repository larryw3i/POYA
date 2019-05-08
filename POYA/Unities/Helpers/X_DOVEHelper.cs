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
using POYA.Data;
using POYA.Models;
using POYA.Unities.Services;
using System;
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
        /// The Guid of public directory
        /// </summary>
        public Guid PublicDirId = Guid.Parse("E1F500C2-FCF4-4BD8-B54A-A2A7A41F793C");

        /// <summary>
        /// env.ContentRootPath + $"/Data/LFiles/Avatars/"
        /// </summary>
        /// <param name="env">
        /// The HostingEnvironment
        /// </param>
        /// <returns></returns>
        public string AvatarStoragePath(IHostingEnvironment env) => env.ContentRootPath + $"/Data/LFiles/Avatars/";

        /// <summary>
        /// env.ContentRootPath + $"/Data/LFiles/"
        /// </summary>
        /// <param name="env">The HostingEnvironment</param>
        /// <returns></returns>
        public string FileStoragePath(IHostingEnvironment env) => env.ContentRootPath + $"/Data/LFiles/";

        /// <summary>
        /// Get the md5 of file byte array
        /// </summary>
        /// <param name="FileBytes">
        /// File byte array
        /// </param>
        /// <returns></returns>
        public string GetFileMD5(byte[] FileBytes)
        {
            var Md5_ = MD5.Create();
            var MD5Bytes = Md5_.ComputeHash(FileBytes);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < MD5Bytes.Length; i++)
            {
                sb.Append(MD5Bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

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
        /// Initial account data for user
        /// </summary>
        /// <param name="context">The ApplicationDbContext</param>
        /// <param name="user">The User</param>
        /// <returns></returns>
        public async Task InitialAccountData(ApplicationDbContext context, IdentityUser user)
        {
            //  var context = ServiceLocator.Instance.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var _X_doveUserInfo = new X_doveUserInfo() { UserId = user.Id };
            await context.X_DoveUserInfos.AddAsync(_X_doveUserInfo);
            await context.LUserMainSharedDirs.AddAsync(new LUserMainSharedDir {UserId=user.Id});
            await context.SaveChangesAsync();
        }

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
                @"
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
                            <td> " + error.Message + @"
                            </td>
                        </tr> 
                        <tr>
                            <td style='min-width:10%'> Data
                            </td>
                            <td> " + error.Data + @"
                            </td>
                        </tr>  
                        <tr>
                            <td style='min-width:10%'> InnerException
                            </td>
                            <td> " + error.InnerException + @"
                            </td>
                        </tr>  
                        <tr>
                            <td style='min-width:10%'> StackTrace
                            </td>
                            <td> " + _stackTrace + @"
                            </td>
                        </tr>
                    </table>"
            #endregion
                );
        }
        #endregion
    }
    public class MimeHelper
    {
        public List<string> GetMime(string FileExtension, IHostingEnvironment env)
        {
            var _CSV = System.IO.File.ReadAllTextAsync(env.ContentRootPath + "/Data/MIME/all_mime.csv").GetAwaiter().GetResult();
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
        }

    }

    /// <summary>
    /// FROM    https://www.cnblogs.com/xishuai/p/asp-net-core-ioc-di-get-service.html
    /// THANK   https://www.cnblogs.com/xishuai/
    /// </summary>
    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }
    }


}
