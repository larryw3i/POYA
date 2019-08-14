

using System;

namespace POYA.Areas.LAdmin.Models{
    // Result
    public class AuditResult{
        
        public Guid Id{get;set;}=Guid.NewGuid();
        
        public Guid ContentId{get;set;}
        
        public bool IsContentLegal{get;set;}=false;

        public string IllegalityType{get;set;}

        public string AuditComment{get;set;}
    }
}