using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.DeveloperZone.Models
{
    public class XDeveloper  //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string HomeCoverImgMD5 { get; set; }
    }
}
