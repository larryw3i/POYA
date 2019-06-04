using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.EduHub.Models
{
    /*
    public class EVideo //  ViewModel
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UseFileId { get; set; }
        public string UserId { get; set; }
        //  public Guid ArticleId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset DOPublish { get; set; }
    }
    */

    public class EArticle
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }

        #region CATEGORY

        /// <summary>
        /// CategoryId include the first and second category, it is  reference from GB/T 13745-2009,
        /// <br/> and there's no guarantee that they're fully compatible,
        /// category of earticle is just a label which author want to set, 
        /// <br/>please let us know if you have a better category 
        /// </summary>
        public Guid CategoryId { get; set; }

        [NotMapped]
        public List<SelectListItem> FirstCategorySelectListItems { get; set; }

        [NotMapped]
        public List<SelectListItem> SecondCategorySelectListItems { get; set; }

        public string AdditionalCategory { get; set; }

        public int ComplexityRank { get; set; } = 0;

        [NotMapped]
        public List<SelectListItem> ComplexityRankSelectListItems { get; set; }

        #endregion

        [StringLength(maximumLength: 50, MinimumLength = 2)]
        public string Title { get; set; }

        [StringLength(maximumLength: 16384)]
        public string Content { get; set; }


        /// <summary>
        /// Determine the article is legal or not by Content appraiser, the default value is <see langword="true"/>
        /// </summary>
        public bool IsLegal { get; set; } = true;

        public DateTimeOffset DOPublishing { get; set; } = DateTimeOffset.Now;

        public DateTimeOffset? DOUpdating { get; set; }

        public long ClickCount { get; set; } = 0;

        #region DEPOLLUTION
        /// <summary>
        /// <see cref="NotMappedAttribute" />
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }

        [NotMapped]
        public long ReaderCount { get; set; } = 0;

        [NotMapped]
        public IEnumerable<IFormFile> LVideos { get; set; }
        [NotMapped]
        public IEnumerable<IFormFile> LAttachments { get; set; }
        #endregion

        #region DISCARD

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
        public string FileMD5 { get; set; }
        public string FileName { get; set; }
        public bool IsEArticleVideo { get; set; } = false;
        #region DEPOLLUTION
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public string ContentType { get; set; }
        #endregion
    }

    public class EArticleFileMD5
    {
        public Guid EArticleId { get; set; }
        public string FileName { get; set;}
        public string MD5 { get; set; }
        public bool IsEArticleVideo { get; set; } = false;
    }

    #region DEPOLLUTION

    public class LEArticleCategory
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    #region ARTICLE_CLASS
    /*
     * Id,Code,Name

     * 
    public class LField
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [StringLength(maximumLength:20)]
        public string Name { get; set; }
    }
    public class LAdvancedClass
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LFieldId { get; set; }
        [StringLength(maximumLength: 20)]
        public string Name { get; set; }
    }
    public class LSecondaryClass
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LAdvancedClassId { get; set; }
        [StringLength(maximumLength: 20)]
        public string Name { get; set; }
    }
    public class LGrade
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LSecondaryClassId { get; set; }
        [StringLength(maximumLength: 20)]
        public string Name { get; set; }
    }
    public class LGradeComment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid LGradeId { get; set; }
        [StringLength(maximumLength: 50)]
        public string Comment { get; set; }

    }
    */
    #endregion


    #endregion
}
