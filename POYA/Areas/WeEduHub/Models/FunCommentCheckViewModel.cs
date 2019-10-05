

using System;

namespace POYA.Areas.WeEduHub.Models
{
    public class FunCommentCheck    //  ViewModel
    {
        public Guid Id{get;set;}
        public Guid FunCommentId{get;set;}
        public bool IsLegal{get;set; }=true;
        public DateTimeOffset DOSubmitting{get;set;}
    }
}