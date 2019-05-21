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
        public string AdminId { get; set; }
        public bool IsIllegal { get; set; } = false;
        public string Comment { get; set; }

    }
}
