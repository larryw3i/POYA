using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using POYA.Data;

namespace POYA.Areas.FunAdmin.Controllers
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


        /// <summary>
        /// Text="Others",Value="110"
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetIllegalityTypeSelectListItems() => new List<SelectListItem>{
            new SelectListItem{Text=_localizer["Pornography | Violence"], Value="0",Selected=true},
            new SelectListItem{Text=_localizer["Politics | Religion"],Value="1"},
            new SelectListItem{Text=_localizer["Instigate"],Value="2"},
            new SelectListItem{Text=_localizer["Discrimination"],Value="3"},
            new SelectListItem{Text=_localizer["Horror | Bloody"],Value="4"},
            new SelectListItem{Text=_localizer["Violate morality"],Value="5"},
            new SelectListItem{Text=_localizer["Others"],Value="110"}
        };

    }
}