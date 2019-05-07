using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.EduHub.Models
{
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
    public class EArticle
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        /// <summary>
        /// The subject id of article, the default value is <see langword="Guid.Empty"/>
        /// </summary>
        public Guid SubjectId { get; set; } = Guid.Empty;
        /// <summary>
        /// The grade id of article, the default value is <see langword="Guid.Empty"/>
        /// </summary>
        public Guid GradeId { get; set; } = Guid.Empty;
        /// <summary>
        /// The type id of article, the default value is <see langword="Guid.Empty"/>
        /// </summary>
        public Guid TypeId { get; set; } = Guid.Empty;
        /// <summary>
        /// The SharedCode of user's video file, the default value is <see langword="Guid.Empty"/>
        /// </summary>
        public Guid VideoSharedCode { get; set; } = Guid.Empty;
        [StringLength(maximumLength: 50)]
        public string Title { get; set; }
        [StringLength(maximumLength:2048)]
        public string Content { get; set; }
        /// <summary>
        /// "text/html" or "text/markdown", the default is "text/html"
        /// </summary>
        public string ContentType { get; set; } = "text/html";
        /// <summary>
        /// Determine the article is legal or not by Content appraiser, the default value is <see langword="true"/>
        /// </summary>
        public bool IsLegal { get; set; } = true;
        public DateTimeOffset DOPublishing { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? DOUpdating { get; set; }
        #region DEPOLLUTION
        /// <summary>
        /// <see cref="NotMappedAttribute" />
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public List<SelectListItem> VideoSharedCodeSelectListItems { get; set; }
        #endregion
    }

    #region DEPOLLUTION
    public class ESubject
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

    }
    public class EGrade
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
    public class EType
    {
        [Key]
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public Guid SubjectId { get; set; }
    }
    #endregion
}
