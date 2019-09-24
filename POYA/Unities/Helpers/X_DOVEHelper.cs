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
    /// <summary>
    /// Some mothods
    /// </summary>
    public class X_DOVEHelper
    {

        public bool IsSafePath(string _Path){
              if (_Path.Contains("..") ||
                _Path.StartsWith('/') ||
                _Path.StartsWith('\\') ||
                _Path.Contains('~') ||
                _Path.Contains("\\\\") ||
                _Path.Contains("//") ||
                _Path.Contains("'") ||
                _Path.Contains("\"")) 
                return false;
            return true;
        }

        /// <summary>
        /// Send error log to email
        /// REFERENCE   https://blog.csdn.net/confused_kitten/article/details/81702861
        /// THANK       https://blog.csdn.net/confused_kitten
        /// </summary>
        /// <param name="context"></param>
        /// <param name="env"></param>
        /// <returns></returns>
        public async Task ErrorEventAsync(IConfiguration configuration, HttpContext context)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            var MarkLineReg = new Regex("(:line\\s?\\d+)");
            var _stackTrace = error.StackTrace;
            var MarkLineMatchs = MarkLineReg.Matches(_stackTrace);

            foreach (var i in MarkLineMatchs)
            {
                _stackTrace = _stackTrace.Replace(
                    oldValue: i.ToString(), 
                    newValue: $"<span style='color:red'><strong>{i}</strong></span>"
                );
            }
            await new EmailSender(configuration).SendEmailAsync(
                email: configuration["ErrorLogHandle:ReceiveLogEmailAddress"],
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
    }

    /// <summary>
    /// Some values
    /// </summary>
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
        public static string AvatarStoragePath(IWebHostEnvironment env) => env.ContentRootPath + $"/Data/LFiles/Avatars/";


        /// <summary>
        ///  env.ContentRootPath + $"/Areas/EduHub/Data/EArticleFiles/"
        /// </summary>
        /// <param name="env">The HostingEnvironment</param>
        /// <returns>  env.ContentRootPath + $"/Areas/EduHub/Data/EArticleFiles/" </returns>
        public static string EduHubFileStoragePath(IWebHostEnvironment env) => env.ContentRootPath + $"/Areas/EduHub/Data/EArticleFiles/";

        /// <summary>
        /// env.ContentRootPath + $"/Data/LFiles/XUserFile/"
        /// </summary>
        /// <param name="env">The HostingEnvironment</param>
        /// <returns></returns>
        public static string FileStoragePath(IWebHostEnvironment env) => env.ContentRootPath + $"/Data/LFiles/XUserFile/";

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
        public static long MaxMultipartBodyLengthLimit = 1024*1024*1024;  // in byte

        /// <summary>
        /// "Administrator"
        /// </summary>
        public static string _administrator = "ADMINISTRATOR";

        public static string CustomHeaderName = "L-XSRF-TOKEN";



    }


}
