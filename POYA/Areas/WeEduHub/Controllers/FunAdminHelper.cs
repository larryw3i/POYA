using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using POYA.Data;

namespace POYA.Areas.WeEduHub.Controllers
{
    public class FunAdminHelper{
        private readonly IStringLocalizer<Program> _localizer;
        private readonly ApplicationDbContext _context;
        
        public FunAdminHelper(
            IStringLocalizer<Program> localizer,
            ApplicationDbContext context
        ){
            _localizer=localizer;
            _context=context;
        }

        public bool IsContentIllegal(Guid Id)
        {
            return 
                _context.FContentCheck.AnyAsync(
                    p=>p.ReceptionistId!=string.Empty && p.ContentId==Id && !p.IsLegal
                )
                .GetAwaiter()
                .GetResult();
        }


        /// <summary>
        /// Text="Others",Value="110"
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetIllegalityTypeSelectListItems() => new List<SelectListItem>{
            new SelectListItem{ Value="0",  Text=$"{_localizer["Pornography"]}|{_localizer["Violence"]}",Selected=true},
            new SelectListItem{ Value="1",  Text=$"{_localizer["Politics"]}|{_localizer["Religion"]}"},
            new SelectListItem{ Value="2",  Text=_localizer["Instigate"]},
            new SelectListItem{ Value="3",  Text=_localizer["Discrimination"]},
            new SelectListItem{ Value="4",  Text=$"{_localizer["Horror"]}|{_localizer["Bloody"]}"},
            new SelectListItem{ Value="5",  Text=_localizer["Violate morality"]},
            new SelectListItem{ Value="110",Text=_localizer["Others"]}
        };

    }
}