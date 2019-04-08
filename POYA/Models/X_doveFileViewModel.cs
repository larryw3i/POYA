using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Models
{
    /// <summary>
    /// Standalone file class associated with file in disk
    /// </summary>
    public class X_doveFile     //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public bool IsLegal { get; set; } = true;
        public Guid OrderId { get; set; } = Guid.NewGuid();
        public long Order { get; set; } = 0;
        public byte[] Hash { get; set; }
        public DateTimeOffset DOUpload { get; set; } = DateTimeOffset.Now;
        public string UploadUserId { get; set; }
    }
    /// <summary>
    /// User/file(or directory) relation, it is directory if FileId is Guid.Empty, or is file, default is directory
    /// </summary>
    public class X_doveUserFileTag
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public Guid FileId { get; set; } = Guid.Empty;
        public string Name { get; set; }
        public DateTimeOffset DOGeneration { get; set; } = DateTimeOffset.Now;
        public Guid InTagId { get; set; } = Guid.Empty;
    }
}
