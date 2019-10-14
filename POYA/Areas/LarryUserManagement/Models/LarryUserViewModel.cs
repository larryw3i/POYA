
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace POYA.Areas.LarryUserManagement.Models
{
    public class LarryUser
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}
        public string Comment{get;set;}

        #region  NotMapped
        
        [NotMapped]
        public string UserName{get;set;}

        [NotMapped]
        public string Email{get;set;}

        [NotMapped]
        public bool IsEmailConfirmed{get;set;}

        #endregion
        
    }
}