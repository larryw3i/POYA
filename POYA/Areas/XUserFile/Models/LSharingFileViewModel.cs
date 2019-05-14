using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XUserFile.Models
{
    public class LSharing   //  File   //  ViewModel
    {
        [Display(Name ="Give it a id for sharing")]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid LUserFileOrDirId { get; set; }

        [StringLength(maximumLength:50)]
        [Display(Name ="Say to receiver")]
        public string Comment { get; set; }
        
        public bool IsLegal { get; set; } = true;

        #region DEPOLLUTION

        [NotMapped]
        public LUserFile LUserFile_ { get; set; }

        [NotMapped]
        public LDir LDir_ { get; set; }

        [NotMapped]
        public bool IsFile_ { get; set; }

        [NotMapped]
        [Display(Name ="Name")]
        public string LUserFileOrDirName_ { get; set; }

        #endregion
    }

    public class SubSharingTemp
    {
        public Guid LSharingDirId { get; set; }
        public Guid LUserFileOrDirId { get; set; }
        public List<SubSharing> SubSharings { get; set; }
    }
    public class SubSharing
    {
        public Guid OriginalId { get; set; }
        public Guid NewId { get; set; } = Guid.NewGuid();
        public Guid OriginalInDirId { get; set;}
        public Guid NewInDirId { get; set; } = Guid.NewGuid();
        public bool IsFile { get; set; }
    }
}
