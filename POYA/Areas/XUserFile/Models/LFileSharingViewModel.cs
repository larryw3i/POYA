using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XUserFile.Models
{
    public class LFileSharing   //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public Guid OrginalId { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset DOSharing { get; set; }
    }
}
