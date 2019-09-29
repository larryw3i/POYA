

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POYA.Areas.WeEduHub.Models
{
    public class WeArticle    //  ViewModel
    {
        public Guid Id{get;set;}
        
        public string AuthorUserId{get;set;}

        public Guid SetId{get;set;}
        
        [Display(Name="Title")]
        [StringLength(maximumLength:120,MinimumLength=2,
        ErrorMessage="The field {0} must be a string with a minimum length of {2} and a maximum length of {1}")]
        public string Title{get;set;}
        
        public Guid WeArticleContentFileId{get;set;}
        
        public DateTimeOffset  DOPublishing{get;set;}=DateTimeOffset.Now;
        
        public DateTimeOffset? DOModifying{get;set;}

        /// <summary>
        /// Id of second class
        /// </summary>
        /// <value></value>
        public Guid ClassId{get;set;}

        [StringLength(maximumLength:100,MinimumLength=2)]
        public string CustomClass{get;set;}

        [Range(1,4)]
        public int  Complex{get;set;}=1;

        [StringLength(maximumLength:100,MinimumLength=0)]
        public string Comment{get;set;}

        [Display(Name="Allow positive sign")]
        public bool IsPositiveSignBeAllowed{get;set;}=false;

        [Display(Name="Allow negative sign")]
        public bool IsNegativeSignBeAllowed{get;set;}=false;

        [Display(Name="Allow comment")]
        public bool IsCommentBeAllowed{get;set;}=false;

        #region  NOTMAPPED

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public string SetName{get;set;}

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public string FirstClassName{get;set;}
        

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public string SecondClassName{get;set;}

        [NotMapped]

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        public List<SelectListItem> ComplexSelectListItems {get;set;}= new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = new string("\u269D") },
                new SelectListItem { Value = "1", Text = new string('\u269D',2) },
                new SelectListItem { Value = "2", Text = new string('\u269D',3)  },
                new SelectListItem { Value = "3", Text = new string('\u269D',4) }
            };


        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public IFormFile WeArticleFormFile{get;set;}

        #endregion
    }
    

}