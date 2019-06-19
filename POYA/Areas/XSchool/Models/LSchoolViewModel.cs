using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XSchool.Models
{

    public class LSchool    //  ViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AdminId { get; set; }
        public string CoverMD5 { get; set; }
        public string Intro { get; set; }
        public DateTimeOffset DORegistering { get; set; }
        public bool IsVerified { get; set; } = false;
        public string Motto { get; set; }
    }
}
