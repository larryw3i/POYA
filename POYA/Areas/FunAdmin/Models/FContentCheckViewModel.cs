
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POYA.Areas.FunAdmin.Models
{
    public class FContentCheck //  ViewModel
    {
        public Guid Id{get;set;}
        
        [Display(Name="Title")]
        public Guid ContentId{get;set;}

        [Display(Name="Appellant")]
        public string AppellantId{get;set;}
        
        [Display(Name="Receptionist")]
        public string ReceptionistId{get;set;}

        [Display(Name="Submitting date")]
        public DateTimeOffset? DOSubmitting{get;set;}

        [Display(Name="Handling date")]
        public DateTimeOffset? DOHandling{get;set;}

        [Display(Name="Appellant comment")]
        public string AppellantComment{get;set;}

        [Display(Name="Receptionist comment")]
        public string ReceptionistComment{get;set;}

        [Display(Name="Legal")]
        public bool IsLegal{get;set;}=true;
        
        [Display(Name="Illegality type")]
        public string IllegalityType{get;set;}

        #region NotMapped
        
        [NotMapped]
        public List<SelectListItem> IllegalityTypeSelectListItems{get;set;}
        
        #endregion
    }
}
