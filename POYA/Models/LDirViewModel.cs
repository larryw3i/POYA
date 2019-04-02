using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Models
{

    /*
     * The new version of thr file system
     */
    public class LDir   //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public Guid InDirId { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public DateTimeOffset DOCreate { get; set; } = DateTimeOffset.Now;
    }
}
