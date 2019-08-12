
using System;

namespace POYA.Areas.LAdmin.Models
{
    // initiative
    public class LAudit{
        public Guid Id{get;set;}=Guid.NewGuid();

        public Guid ContentId{get;set;}

        public DateTimeOffset DOAudit{get;set;}=DateTimeOffset.Now;

        public string InspectorId{get;set;}
        
        public Guid AuditResultId{get;set;}
        
    }
}