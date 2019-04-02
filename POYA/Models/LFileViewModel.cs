using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Models
{
    public class LFile:LDir  //  ViewModel
    {
        public byte[] Buffer { get; set; }
        public string ContentType { get; set; }
        public long FileNodeOrder { get; set; } = 0;
    }
}
