using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XUserFile.Models
{
    public class LSharingFile   //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid LUserFileOrDirId { get; set; }

        [StringLength(maximumLength:50)]
        public string Comment { get; set; }
        
        public bool IsLegal { get; set; } = true;
    }
}
