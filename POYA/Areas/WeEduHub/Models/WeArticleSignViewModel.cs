
using System;

namespace POYA.Areas.WeEduHub.Models
{
    public class WeArticleSign  //  ViewModel
    {
        public Guid Id{get;set;}
        public Guid WeArticleId{get;set;}
        public bool IsPositive{get;set;}=true;
        public string UserId{get;set;}
        public DateTimeOffset DOSigning{get;set;}=DateTimeOffset.Now;
    }
}