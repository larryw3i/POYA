
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POYA.Areas.LarryUserManagement.Models
{
    public class LarryUser
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}
        [Display(Name = "Comment")]
        public string Comment{get;set;}

        #region  NotMapped
        
        [NotMapped]
        [Display(Name = "User name")]
        [StringLength(maximumLength:50,MinimumLength=1)]
        public string UserName{get;set;}

        [Required]
        [NotMapped]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email{get;set;}

        [NotMapped]
        [Display(Name = "Is email confirmed")]
        public bool IsEmailConfirmed{get;set;}=true;


        [Required]
        [NotMapped]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match")]
        public string ConfirmPassword { get; set; }
        

        [NotMapped]
        [StringLength(maximumLength:25)]
        [Display(Name = "Telphone number")]
        public string TelphoneNumber{get;set;}

        [NotMapped]
        public List<SelectListItem> RoleSelectListItems{get;set;} = new List<SelectListItem>{
            new SelectListItem{
                Text = ""
            }
        };

        #endregion
        
    }
}