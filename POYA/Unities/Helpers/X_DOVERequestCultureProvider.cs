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
            var CultureCookie = httpContext.Request.Cookies["CULTURE"] == null ? "" : httpContext.Request.Cookies["CULTURE"].ToString();
            var CultureQueryString = httpContext.Request.Query["culture"].ToString();
            if (CultureQueryString != null && CultureQueryString != "")
            {
                if (CultureQueryString != CultureCookie)
                {
                    httpContext.Response.Cookies.Append(key: "CULTURE", value: CultureQueryString,
                        options: new CookieOptions() { Expires = DateTime.Now.AddYears(1) });
                }
                return Task.FromResult(new ProviderCultureResult(CultureQueryString));
            }
            if (CultureCookie == "") return Task.FromResult(new ProviderCultureResult("zh-CN"));
            return Task.FromResult(new ProviderCultureResult(CultureCookie));
        }
    }
}
