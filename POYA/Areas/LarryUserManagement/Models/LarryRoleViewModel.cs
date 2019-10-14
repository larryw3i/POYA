
using System;
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
        public string RoleName{get;set;}

        [NotMapped]
        public string ROleNormalizedName{get;set;}

        #endregion
    }
}