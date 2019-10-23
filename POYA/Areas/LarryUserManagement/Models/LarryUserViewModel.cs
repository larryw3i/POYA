
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using POYA.Unities.Helpers;

namespace POYA.Areas.LarryUserManagement.Models
{
    public class LarryUser
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}
        [Display(Name = "Comment")]
        public string Comment{get;set;}

        #region  NotMapped
        
        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        [Display(Name = "User name")]
        [StringLength(maximumLength:50,MinimumLength=1)]
        public string UserName{get;set;}

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [Required]
        [NotMapped]
        [EmailAddress]
        [Display(Name = "Email")]
        [Remote(action: "RepetitionEmailCheck", controller: "LarryUsers", ErrorMessage = "This email is used")]
        public string Email{get;set;}

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        [Display(Name = "Is email confirmed")]
        public bool IsEmailConfirmed{get;set;}=true;


        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [Required]
        [NotMapped]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
        

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        [StringLength(maximumLength:25)]
        [Display(Name = "Telphone number")]
        public string TelphoneNumber{get;set;}

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public List<SelectListItem> RoleSelectListItems{get;set;} 

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public Guid RoleId{get;set;} 

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public string RoleName{get;set;} 

        #endregion
        
    }
}