using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace POYA.Unities.Helpers
{
    public class X_DOVERequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var CULTURE_String="CULTURE";
            var CultureCookie = httpContext.Request.Cookies[CULTURE_String]?.ToString() ?? ""; 
            // if (CultureCookie.StartsWith('-')) 
            // {
            //     CultureCookie = CultureCookie.Substring(1);
            //     httpContext.Response.Cookies.Append(key: CULTURE_String, value: CultureCookie,options: new CookieOptions() { Expires = DateTime.Now.AddYears(1) });
            // }else 
            if (string.IsNullOrWhiteSpace(CultureCookie))
            {
                CultureCookie = "zh-Hans";
                httpContext.Response.Cookies.Append(key:CULTURE_String, value:CultureCookie, options: new CookieOptions() { Expires = DateTime.Now.AddYears(1) });
            }
            return Task.FromResult(new ProviderCultureResult(CultureCookie));
        }
    }
}
