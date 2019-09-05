using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using POYA.Unities.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.EduHub.Models
{
    public class EArticle
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid(); 

        public string UserId { get; set; }

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }
        

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public string UserEmail { get; set; }

 
        /// <summary>
        /// The EArticle set id
        /// </summary>
        public Guid SetId { get; set; }     //  = LValue.DefaultEArticleSetId;

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public string SetName { get; set; }


        /// <summary>
        /// CategoryId include the first and second category, it is  reference from GB/T 13745-2009,
        /// <br/> and there's no guarantee that they're fully compatible,
        /// category of earticle is just a label which author want to set, 
        /// <br/>please let us know if you have a better category 
        /// </summary>
        public Guid CategoryId { get; set; }

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public List<SelectListItem> FirstCategorySelectListItems { get; set; }

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public List<SelectListItem> SecondCategorySelectListItems { get; set; }

        public string AdditionalCategory { get; set; }
 
        [Range(0,3)] 
        public int ComplexityRank { get; set; } = 0;
 

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped] 
        public List<SelectListItem> ComplexityRankSelectListItems { get; set; }
 
        [StringLength(maximumLength: 50, MinimumLength = 2)] 
        public string Title { get; set; }
 
        [StringLength(maximumLength: 16384)] 
        public string Content { get; set; }
 
        /// <summary>
        /// Determine the article is legal or not by Content appraiser, the default value is true/>
        /// </summary> 
        public bool IsLegal { get; set; } = true;

        public DateTimeOffset DOPublishing { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? DOUpdating { get; set; }

        public long ClickCount { get; set; } = 0;

        #region DEPOLLUTION

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public long ReaderCount { get; set; } = 0;

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public IEnumerable<IFormFile> LVideos { get; set; }

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public IEnumerable<IFormFile> LAttachments { get; set; }


        /// <summary>
        /// || DISCARD ||
        /// </summary>
        public Guid ClassId { get; set; }

        /// <summary>
        /// || DISCARD ||
        /// </summary>
        public Guid LGradeId { get; set; }

        /// <summary>
        /// || DISCARD ||
        /// "text/html" or "text/markdown", the default is "text/html"
        /// </summary>
        public string ContentType { get; set; } //    = "text/html";
        #endregion
    }

    public class EArticleFile
    {

        /// <summary>
        /// The default is  Guid.NewGuid()
        /// </summary> 
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EArticleId { get; set; }
        public string FileSHA256 { get; set; }
        public string FileName { get; set; }
        public bool IsEArticleVideo { get; set; } = false;

        #region NOTMAPPED

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public string ContentType { get; set; }

        #endregion
    
    }

    public class EArticleFileSHA256
    {
        public Guid EArticleId { get; set; }
        public string FileName { get; set; }
        public string SHA256 { get; set; }
        public bool IsEArticleVideo { get; set; } = false;
    }

    #region DEPOLLUTION

    public class LEArticleCategory
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }


    #endregion
}
