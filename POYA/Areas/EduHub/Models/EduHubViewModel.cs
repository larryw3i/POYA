using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.EduHub.Models
{
    public class EVideo //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UseFileId { get; set; }
        public string UserId { get; set; }
        //  public Guid ArticleId { get; set; }
        public string Title { get; set; }
        public DateTimeOffset DOPublish { get; set; }
    }
    public class EArticle
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public int SubjectId { get; set; }
        public int GradeId { get; set; }
        public int TypeId { get; set; }
        /// <summary>
        /// It's default value is Guid.Empty
        /// </summary>
        public Guid VideoId { get; set; } = Guid.Empty;
        public string Title { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// text/html | text/markdown
        /// </summary>
        public string ContentType { get; set; } = "text/html";
        public bool IsLegal { get; set; } = true;
    }

    #region DEPOLLUTION
    public class ESubject
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

    }
    public class EGrade
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
    }
    public class EType
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public Guid SubjectId { get; set; }
    }
    #endregion
}
