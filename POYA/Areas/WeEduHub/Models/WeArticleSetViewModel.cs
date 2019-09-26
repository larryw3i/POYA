

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POYA.Areas.WeEduHub.Models
{
    public class WeArticleSet
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}
        public Guid CoverFileId{get;set;}
        [Display(Name="Name")]
        public string Name{get;set;}
        [Display(Name="description")]
        public string  Description{get;set;}
        public DateTimeOffset DOCreating{get;set;}=DateTimeOffset.Now;
        
        #region  NOTMAPPED
        #endregion
    }

}