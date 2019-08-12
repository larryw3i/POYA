
using System;

namespace POYA.Areas.LAdmin.Models
{
    // passive
    public class UserComplaint{
        public Guid Id{get;set;}=Guid.NewGuid();

        public string UserId{get;set;}

        public Guid ContentId{get;set;}

        public DateTimeOffset DOComplaint{get;set;}=DateTimeOffset.Now;

        public string ReceptionistId{get;set;}

        public Guid AuditResultId{get;set;}

        public string Description{get;set;}

        public int IllegalityType{get;set;}
    }
}