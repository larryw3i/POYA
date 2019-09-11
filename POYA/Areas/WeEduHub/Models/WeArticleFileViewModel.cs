

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace POYA.Areas.WeEduHub.Models
{
    public class WeArticleFile
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}

        /// <summary>
        /// For video file
        /// </summary>
        /// <value></value>
        public Guid? WeArticleId{get;set;}

        /// <summary>
        /// With extension
        /// </summary>
        /// <value></value>
        public string Name{get;set;}

        public DateTimeOffset DOUploading{get;set;}=DateTimeOffset.Now;

        #region  NOTMAPPED

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public string ContentType{get;set;}="application/octet-stream";
        
        #endregion
    }
}