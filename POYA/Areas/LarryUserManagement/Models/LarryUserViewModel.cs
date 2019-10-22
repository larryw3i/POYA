
using System;
using System.ComponentModel.DataAnnotations;
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
        [StringLength(maximumLength:50,MinimumLength=1)]
        public string UserName{get;set;}

        [Required]
        [NotMapped]
        [EmailAddress]
        public string Email{get;set;}

        [NotMapped]
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

        #endregion
        
    }
}