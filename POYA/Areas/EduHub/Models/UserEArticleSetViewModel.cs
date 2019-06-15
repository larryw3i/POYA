using CsvHelper.Configuration.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.EduHub.Models
{
    public class UserEArticleSet   //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }

        #region 
        [Display(Name = "Title")]
        [StringLength(maximumLength: 50, MinimumLength = 2)]
        #endregion
        public string Name { get; set; }

        #region 

        [Display(Name = "Label")]
        [StringLength(maximumLength: 20, MinimumLength = 2)]
        #endregion
        public string Label { get; set; }

        #region
        /// <summary>
        /// The default is DateTimeOffset.Now
        /// </summary>
        [Display(Name="Date of creating")]
        #endregion
        public DateTimeOffset DOCreating { get; set; } = DateTimeOffset.Now;

        #region 

        [Display(Name = "Comment")]
        [StringLength(maximumLength: 20, MinimumLength = 2)]
        #endregion
        public string Comment { get; set; }
    }

    public class UserEArticleHomeInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string CoverFileMD5 { get; set; }
        public string Comment { get; set; }

        public string CoverFileContentType { get; set; } = "image/png";

        #region DEPOLLUTION

        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        #endregion
        [NotMapped]
        public IFormFile CoverFile { get; set; }
        #endregion
    }
}
