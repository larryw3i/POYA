
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POYA.Areas.LAdmin.Models
{
    // passive
    public class UserComplaint{
        public Guid Id{get;set;}=Guid.NewGuid();

        public string UserId{get;set;}

        public Guid ContentId{get;set;}

        public DateTimeOffset DOComplaint{get;set;}=DateTimeOffset.Now;

        public string ReceptionistId{get;set;}

        public Guid AuditResultId{get;set;}

        [Display(Name="Description")]
        public string Description{get;set;}

        [Display(Name="IllegalityType")]
        public string IllegalityType{get;set;}

        
        #region NotMapped

        [NotMapped]
        public List<SelectListItem> IllegalityTypeSelectListItems{get;set;}

        #endregion

        
    }
}