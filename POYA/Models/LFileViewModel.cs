using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Models
{
    public class LUserFile  //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The MD5 string, it equal the file name in specified directory
        /// </summary>
        public string MD5 { get; set; }
        public string UserId { get; set; }
        /// <summary>
        /// It is shared if SharedCode isn't Guid.Empty
        /// </summary>
        public Guid SharedCode { get; set; } = Guid.Empty;
    }
    public class LFile
    {
        public Guid Id { get; set; } = Guid.Empty;
        public string Upload_UserId { get; set; }
        public DateTimeOffset DOUpload { get; set; } = DateTimeOffset.Now;
        public string MD5 { get; set; }
    }
}
