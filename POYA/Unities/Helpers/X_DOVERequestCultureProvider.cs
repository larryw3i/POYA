using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace POYA.Unities.Helpers
{
    public class X_DOVERequestCultureProvider : RequestCultureProvider
    {
        public override Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var CULTURE_String="CULTURE";
            var CultureCookie = httpContext.Request.Cookies[CULTURE_String]?.ToString() ?? "";    
            var _SupportedUICultures = httpContext.RequestServices
                                        .GetService<IOptions<RequestLocalizationOptions>>()
                                        .Value.SupportedUICultures.Select(c => c.Name).ToList();
            if (string.IsNullOrWhiteSpace(CultureCookie)|| !_SupportedUICultures.Contains(CultureCookie))
            {
                CultureCookie = "zh-Hans";
                httpContext.Response.Cookies.Append(key:CULTURE_String, value:CultureCookie, options: new CookieOptions() { Expires = DateTime.Now.AddYears(1) });
            }
            return Task.FromResult(new ProviderCultureResult(CultureCookie));
        }
    }
}
