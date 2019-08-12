using System;
using System.ComponentModel.DataAnnotations;

namespace POYA.Areas.XAd.Models
{
    public class LAd
    {
        public Guid Id{get;set;}=Guid.NewGuid();
        public string AdvertiserUserId{get;set;}

        [Display(Name="Title")]
        [StringLength(maximumLength:50)]
        public string Title{get;set;}

        [Display(Name="Details")]
        [StringLength(maximumLength:1024)]
        public string Content{get;set;}

        public DateTimeOffset DOPublishing{get;set;}=DateTimeOffset.Now;
    }

    public class LAdImage{
        public Guid Id{get;set;}=Guid.NewGuid();
        public string SHA256{get;set;}
        public string ContentType{get;set;}

    }
    
    public class UserFeedback{
        public Guid Id{get;set;}=Guid.NewGuid();
        public string UserId{get;set;}
        
        [Display(Name="Date of feedback")]
        public DateTimeOffset DOFeedback{get;set;}=DateTimeOffset.Now;

        [Display(Name="Comment")]
        [StringLength(maximumLength:128)]
        public string Comment{get;set;}
        
    }
    public class UserFeedbackImage{
        public Guid Id{get;set;}=Guid.NewGuid();
        public string SHA256{get;set;}
        public string ContentType{get;set;}
    }
}