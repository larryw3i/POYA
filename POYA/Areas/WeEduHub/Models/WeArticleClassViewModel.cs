
using System;

namespace POYA.Areas.WeEduHub.Models
{
    public class WeArticleFirstClass //  ViewModel
    {
        public Guid Id{get;set;}
        public string Code{get;set;}
        public string Name{get;set;}
    }

    public class WeArticleSecondClass:WeArticleFirstClass //  ViewModel
    {
    }
}