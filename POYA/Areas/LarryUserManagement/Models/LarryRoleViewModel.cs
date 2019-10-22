
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace POYA.Areas.LarryUserManagement.Models
{
    public class LarryRole
    {

        public Guid Id{get;set;}
        public string RoleId{get;set;}
        public string Comment{get;set;}

        #region  NotMapped

        [NotMapped]
        [StringLength(maximumLength:20)]
        public string RoleName{get;set;}

        [NotMapped]
        [StringLength(maximumLength:20)]
        public string ROleNormalizedName{get;set;}

        #endregion
    }
}