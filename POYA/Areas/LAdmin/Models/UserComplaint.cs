
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POYA.Areas.LAdmin.Models
{
    // passive
    public class UserComplaint: AuditCommon{
        public Guid Id{get;set;}=Guid.NewGuid();

        public string  ComplainantId{get;set;}

        public Guid ContentId{get;set;}

        [Display(Name="Date")]
        public DateTimeOffset DOComplaint{get;set;}=DateTimeOffset.Now;

        [Display( Name="Revised on")]
        public DateTimeOffset? DOModifying{get;set;}

        public string ReceptionistId{get;set;}

        public Guid AuditResultId{get;set;}

        [Display(Name="Description")]
        public string Description{get;set;}

        [Display(Name="Illegality type")]
        public string IllegalityType{get;set;}

        
        #region NotMapped

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        [Display(Name="Type")]
        public string IllegalityTypeString{get;set;}


        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        [Display(Name="Receptionist")]
        public string ReceptionistName{get;set;}

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        [Display(Name="Complainant")]
        public string ComplainantName{get;set;}

        [NotMapped]
        [Display(Name="Title")]
        public string ContentTitle{get;set;}

        #endregion

        
    }
}