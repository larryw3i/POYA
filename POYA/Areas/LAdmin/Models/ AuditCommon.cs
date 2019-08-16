
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POYA.Areas.LAdmin.Models{
    public class  AuditCommon{

        #region NotMapped

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public List<SelectListItem> IllegalityTypeSelectListItems{get;set;}

        #endregion
    }
}