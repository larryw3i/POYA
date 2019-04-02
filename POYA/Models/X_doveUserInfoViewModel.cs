using Microsoft.AspNetCore.Identity;
using POYA.Unities.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Models
{
    [PersonalData]
    public class X_doveUserInfo
    {
        [Key]
        public string UserId { get; set; }

        public byte[] AvatarBuffer { get; set; } = new byte[0];

        [DOBirth(IsValueNullable = true)]
        [Display(Name = "Birth date")]
        public DateTimeOffset? DOBirth { get; set; }

        [Display(Name = "Overt")]
        public bool IsDOBirthOvert { get; set; } = false;

        [Display(Name = "Hobby")]
        [StringLength(maximumLength: 50)]
        public string Hobby { get; set; }

        [Display(Name = "Overt")]
        public bool IsHobbyOvert { get; set; } = false;

        [Display(Name = "Your hometown")]
        [StringLength(maximumLength: 100)]
        public string OriginalAddress { get; set; }

        [Display(Name = "Overt")]
        public bool IsOriginalAddressOvert { get; set; } = false;

        [Display(Name = "Your address")]
        [StringLength(maximumLength: 100)]
        public string Address { get; set; }

        [Display(Name = "Overt")]
        public bool IsAddressOvert { get; set; } = false;

        [Display(Name = "Marital status")]
        [StringLength(maximumLength: 10)]
        public string MaritalStatus { get; set; }

        [Display(Name = "Overt")]
        public bool IsMaritalStatusOvert { get; set; }

        [Display(Name = "Graduated from")]
        [StringLength(maximumLength: 50)]
        public string GraduatedFrom { get; set; }

        [Display(Name = "Overt")]
        public bool IsGraduatedFromOvert { get; set; } = false;

        [Display(Name = "Occupation")]
        [StringLength(maximumLength: 50)]
        public string Occupation { get; set; }

        [Display(Name = "Overt")]

        public bool IsOccupationOvert { get; set; } = false;

    }
     
}
