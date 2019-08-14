

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.DependencyInjection;
using POYA.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace POYA.Areas.LAdmin.Controllers{


    public class LAdminHelper{
        private readonly IStringLocalizer<Program> _localizer;
        private readonly ApplicationDbContext _context;
        
        public LAdminHelper(
            IStringLocalizer<Program> localizer,
            ApplicationDbContext context
        ){
            _localizer=localizer;
            _context=context;
        }

        public async Task<string> GetContentTitleAsync(Guid Id) 
            => await _context.EArticle.Where(p => p.Id == Id).Select(p => p.Title).FirstOrDefaultAsync();

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