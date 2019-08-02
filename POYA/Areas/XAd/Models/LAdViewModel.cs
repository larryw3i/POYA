using System;

namespace POYA.Areas.XAd.Models
{
    public class LAd
    {
        public Guid Id{get;set;}=Guid.NewGuid();
        public string AdvertiserUserId{get;set;}
        public string Title{get;set;}
        public string Content{get;set;}
        public DateTimeOffset DOPublishing{get;set;}=DateTimeOffset.Now;
    }

    public class LAdImage{
        public Guid Id{get;set;}=Guid.NewGuid();
        public string MD5{get;set;}
        public string ContentType{get;set;}

    }
    public class UserFeedback{
        public Guid Id{get;set;}=Guid.NewGuid();
        public string UserId{get;set;}
        public DateTimeOffset DOFeedback{get;set;}=DateTimeOffset.Now;
        public string Comment{get;set;}
        
    }
    public class UserFeedbackImage{
        public Guid Id{get;set;}=Guid.NewGuid();
        public string MD5{get;set;}
        public string ContentType{get;set;}
    }
}