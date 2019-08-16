

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POYA.Areas.LAdmin.Models{
    // Result
    public class AuditResult:AuditCommon{
        
        public Guid Id{get;set;}=Guid.NewGuid();
        
        public Guid ComplainantId{get;set;}
        
        [Display(Name="Is content legal")]
        public bool IsContentLegal{get;set;}=false;

        [Display(Name="Illegality type")]
        public string IllegalityType{get;set;}

        [Display(Name="Comment")]
        public string AuditComment{get;set;}

        #region NotMapped

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public Guid ContentId{get;set;}

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        [Display(Name="Title")]
        public string ContentTitle{get;set;}


        #endregion
    }
}