using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Models
{
    public class SharedFDCommon   //  ViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTimeOffset DOSharing { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? DOEnd { get; set; }
        public string Tip { get; set; }
        [NotMapped]
        public string UserId_ { get; set; }
        
        [NotMapped]
        public string Name_{get;set;}

        [NotMapped]
        public string ShringUserName_{get;set;}

    }
    public class SharedF:SharedFDCommon
    {
        public Guid FileId { get; set; } 
        [NotMapped]
        public long Size_{get;set;}
        [NotMapped]
        public string ContentType_{get;set;}
        [NotMapped]
        public bool IsGetSharing_{get;set;}=false;
    }
    public class SharedD : SharedFDCommon
    {
        public Guid DirId { get; set; }
    }
    public class GetSharedD
    {
        public List<X_doveDirectory> X_DoveDirectories { get; set; }
        public List<X_doveFile> X_DoveFiles { get; set; }
    }

    public class DirSubFDId
    {
        public Guid InDirId { get; set; }
        public Guid FoDId { get; set; }
    }
}
