
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace POYA.Areas.WeEduHub.Models
{
    public class FunComment //  ViewModel
    {
        public Guid Id{get;set;}
        public string CommentUserId{get;set;}
        public Guid WeArticleId{get;set;}
        
        [Display(Name="Comment")]
        public string CommentContent{get;set;}
        public DateTimeOffset DOCommenting{get;set;}=DateTimeOffset.Now;
        public bool IsShielded{get;set;}=false;
        public DateTimeOffset? DOShielding{get;set;}

        #region     DEPOLLUTION
        
        [NotMapped]
        public string CommentUserName{get;set;}

        #endregion

    }
}