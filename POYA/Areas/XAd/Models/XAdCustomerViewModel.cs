using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XAd.Models
{
    public class XAdCustomer    //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        #region STORE_ICON

        public string StoreIconSHA256 { get; set; }

        public string StoreIconContentType { get; set; }
        
        #region 

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        [Display(Name="Store icon")]
        #endregion
        public IFormFile StoreIconFile { get; set; }
        #endregion

        #region 

        [Display(Name = "Store name")]
        [StringLength(maximumLength:20,MinimumLength =2)]
        #endregion
        public string Name { get; set; }

        #region USER
        public string UserId { get; set; }
        
        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        /// <value></value>
        [NotMapped]
        [Display(Name="Registrant")]
        #endregion
        public string UserName{get;set;}

        #endregion

        #region  LICENSE_FILES

        #region 

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        [Display(Name = "Upload license images")]
        #endregion
        public List<IFormFile> LicenseImgFiles { get; set; }
        #endregion

        [Display(Name="Date of registering")]
        public DateTimeOffset DORegistering { get; set; } = DateTimeOffset.Now;

        #region 

        [Display(Name = "Business address")]
        [StringLength(maximumLength: 80, MinimumLength = 2)]
        #endregion
        public string Address { get; set; }

        #region 
        /// <summary>
        /// 
        /// </summary>
        [Display(Name= "Introduction")]
        [StringLength(maximumLength:128)]
        #endregion
        public string Intro { get; set; }

        /// <summary>
        /// For regulator, industry and commerce department, and so on
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        #region 
        [NotMapped]
        #endregion
        public List<Guid> WillBeDeletedLicenseImgIds{get;set;}
    }

    public class XAdCustomerLicense
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string XAdCustomerUserId { get; set; }

        public string ImgFileSHA256 { get; set; }

        public string ImgFileContentType { get; set; }

        #region     
        /// <summary>
        /// [NotMapped]<br/>
        /// DISCARD
        /// </summary>
        [NotMapped]
        #endregion
        public Guid XAdCustomerId { get; set; }

        #region DISCARD

        #region 
        /// <summary>
        /// DISCARD
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public Guid LicenseImgFileId { get; set; }

        #endregion
    }

    public class UserPraiseOr{
        public Guid Id{get;set;}=Guid.NewGuid();
        public string UserId{get;set;}
        public bool IsPraise{get;set;}=true;
    }

}
