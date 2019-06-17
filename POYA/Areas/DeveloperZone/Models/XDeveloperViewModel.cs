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
        public DateTimeOffset DOJoining { get; set; } = DateTimeOffset.Now;
    }

    public class XDeveloperNote
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid XDeveloperId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTimeOffset DOPublishing { get; set; } = DateTimeOffset.Now;

    }
}
