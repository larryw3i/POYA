using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using POYA.Unities.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace POYA.Unities.Helpers
{
    public class X_DOVEHelper
    {
        public async Task ErrorEventAsync(HttpContext context, IHostingEnvironment env)
        {
            var feature = context.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;

            var file = File.OpenText(env.ContentRootPath+"/appsettings.json");
            var reader = new JsonTextReader(file);
            var jsonObject = (JObject)JToken.ReadFrom(reader);
            file.Close();

            var MarkLineReg = new Regex("(:line\\s?\\d+)");
            var _stackTrace = error.StackTrace; 
            var MarkLineMatchs = MarkLineReg.Matches(_stackTrace); 
            foreach(var i in MarkLineMatchs)
            {
                _stackTrace= _stackTrace.Replace(oldValue:i.ToString(),newValue: $"<span style='color:red'><strong>{i}</strong></span>"); 
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

    }
}
