using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XLaw.Models
{
    public class Complaint  //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ToId { get; set; }
        public string FromUserId { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset DOSubmitting { get; set; } = DateTimeOffset.Now;
    }

    public class Arbitrament
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ComplaintId { get; set; }
        public string AdminId { get; set; }
        public DateTimeOffset DOArbitrament { get; set; } = DateTimeOffset.Now;
        public string AdminComment { get; set; }
        public bool IsComplaintContentLegal { get; set; } = true;
    }
}
