

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
        public string UserId{get;set;}
        public Guid SetId{get;set;}
        
        [StringLength(maximumLength:120,MinimumLength=2)]
        public string Title{get;set;}
        public Guid WeArticleFileId{get;set;}
        public DateTimeOffset  DOPublishing{get;set;}=DateTimeOffset.Now;
        public DateTimeOffset? DOModifying{get;set;}

        public Guid ClassId{get;set;}

        public string CustomClass{get;set;}

        public int  Complex{get;set;}=1;

        #region  NOTMAPPED

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

        /// <summary>
        /// NotMapped
        /// </summary>
        [NotMapped]
        public string Content{get;set;}
        #endregion
    }
    
    public class WeArticleSet
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}
        public Guid CoverFileId{get;set;}
        public string Name{get;set;}
        public string  Description{get;set;}
        public DateTimeOffset DOCreating{get;set;}=DateTimeOffset.Now;
        
        #region  NOTMAPPED
        #endregion
    }

    public class WeArticleFile
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}

        /// <summary>
        /// With extension
        /// </summary>
        /// <value></value>
        public string Name{get;set;}
        public DateTimeOffset DOUploading{get;set;}=DateTimeOffset.Now;

        #region  NOTMAPPED
        #endregion
    }
}