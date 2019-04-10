using Microsoft.AspNetCore.Http;
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
        public byte[] Hash { get; set; }    //  MD5
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
    
    /// <summary>
    /// The class of file md5 returned from browser
    /// </summary>
    public class BFileMD5
    {
        public IEnumerable<FileInfo_> FileInfos_ { get; set; }
        public Guid InTagId { get; set; }
    }
    
    /// <summary>
    /// The result of contrast file's md5
    /// </summary>
    public class FileMD5ContrastResult
    {
        public int Id { get; set; }
        /// <summary>
        /// Default value is false
        /// </summary>
        public bool IsUploaded { get; set; } = false;
    }

    /// <summary>
    /// The Id and MD5 for FileMD5ContrastResult
    /// </summary>
    public class FileInfo_
    {
        public int Id { get; set; }
        public string MD5 { get; set; }
        public string FileName { get; set; }
    }

    public class X_DoveFile_Hash_String
    {
        public Guid Id { get; set; }
        public string HashString { get; set; }
    }


    /// <summary>
    /// Slice file
    /// </summary>
    public class SliceLFile
    {
        public IFormFile LFile { get; set; }
        public bool IsLast { get; set; } = false;
        /// <summary>
        /// 0~100
        /// </summary>
        public long Progress_ { get; set; } = 0;
    }
}
