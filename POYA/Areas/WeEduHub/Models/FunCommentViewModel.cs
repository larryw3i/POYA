
using System;

namespace POYA.Areas.WeEduHub.Models
{
    public class FunComment //  ViewModel
    {
        public Guid Id{get;set;}
        public string CommentUserId{get;set;}
        public Guid WeArticleId{get;set;}
        public string CommentContent{get;set;}
        public DateTimeOffset DOCommenting{get;set;}=DateTimeOffset.Now;
        public bool IsShielded{get;set;}=false;
        public DateTimeOffset? DOShielding{get;set;}

    }
}