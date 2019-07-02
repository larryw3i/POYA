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

        public string StoreIconMD5 { get; set; }

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
        #endregion
        public string Name { get; set; }

        public string UserId { get; set; }

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

        public DateTimeOffset DORegistering { get; set; } = DateTimeOffset.Now;

        #region 

        [Display(Name = "Business address")]
        #endregion
        public string Address { get; set; }
    }

    public class XAdCustomerLicense
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string XAdCustomerUserId { get; set; }

        public string ImgFileMD5 { get; set; }

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

    public class XAdCustomerVerification
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid XAdCustomerId { get; set; }
        public string VerificationAdminId { get; set; }
        public DateTimeOffset DOVerification { get; set; } = DateTimeOffset.Now;
        public bool IsVerified { get; set; } = false;
    }

}
