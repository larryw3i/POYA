
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace POYA.Areas.FunAdmin.Models
{
    public class FContentCheck //  ViewModel
    {
        public Guid Id{get;set;}
        
        public Guid ContentId{get;set;}

        public string AppellantId{get;set;}
        
        public string ReceptionistId{get;set;}

        public DateTimeOffset? DOSubmitting{get;set;}

        public DateTimeOffset? DOHandling{get;set;}

        public string AppellantContent{get;set;}

        public string ReceptionistContent{get;set;}

        public bool IsLegal{get;set;}=true;

        #region NotMapped

        public string ContentTitle{get;set;}

        #endregion
    }
}
