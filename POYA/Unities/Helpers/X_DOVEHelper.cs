using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
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

        public async Task ErrorEventAsync(HttpContext context, IHostingEnvironment env)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            var file = File.OpenText(env.ContentRootPath + "/appsettings.json");
            var reader = new JsonTextReader(file);
            var jsonObject = (JObject)JToken.ReadFrom(reader);
            file.Close();
            var MarkLineReg = new Regex("(:line\\s?\\d+)");
            var _stackTrace = error.StackTrace;
            var MarkLineMatchs = MarkLineReg.Matches(_stackTrace);
            foreach (var i in MarkLineMatchs)
            {
                _stackTrace = _stackTrace.Replace(oldValue: i.ToString(), newValue: $"<span style='color:red'><strong>{i}</strong></span>");
            }
            await new EmailSender(env).SendEmailAsync(
                email: (string)jsonObject[nameof(X_DOVEHelper)]["email"],
                subject: "X_DOVE_ERROR",
                htmlMessage:
            #region
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
            #region
            /*
             * * LogHelper.Write("Global\\Error", error.Message, error.StackTrace);
             * * return context.Response.WriteAsync(JsonHelper.ToJson(new RequestResult(444, "系统未知异常，请联系管理员")), Encoding.GetEncoding("GBK")); 
             * * --------------------- 
             * * 作者：confused_kitten
             * * 来源：CSDN
             * * 原文：https://blog.csdn.net/confused_kitten/article/details/81702861 
             * * 版权声明：本文为博主原创文章，转载请附上博文链接！
             */
            #endregion
        }

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
        /// Get the full path of file or directory, the id is ignored if InDirId is passed
        /// </summary>
        /// <param name="id">The id of file or directory</param>
        /// <param name="context">The context</param>
        /// <param name="InDirId">The id of directory contain file or directory</param>
        /// <returns></returns>
        public string GetFullPathOfFileOrDir(ApplicationDbContext context, Guid InDirId)
        {
            var FullPath = string.Empty;
            for (var i = 0; i < 30 && InDirId != Guid.Empty; i++)
            {
                var InDir =  context.LDir.Where(p => p.Id == InDirId).Select(p => new { p.InDirId, p.Name }).FirstOrDefaultAsync()
                    .GetAwaiter().GetResult();
                if (InDir?.InDirId== null) break;
                FullPath = $"{InDir.Name}/{FullPath}";
                InDirId = InDir.InDirId;
            }
            return $"root/{FullPath}";
        }
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

}
