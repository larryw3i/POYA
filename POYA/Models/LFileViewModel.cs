using Microsoft.AspNetCore.Http;
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
        /// The MD5 string, it equal the file name in specified directory, and set to null after finishing slice file upload 
        /// </summary>
        public string MD5 { get; set; } = string.Empty;
        public string UserId { get; set; }
        /// <summary>
        /// It is shared if SharedCode isn't Guid.Empty
        /// </summary>
        public Guid SharedCode { get; set; } = Guid.Empty;
        public DateTimeOffset DOGenerating { get; set; } = DateTimeOffset.Now;
        public string Name { get; set; }
        public Guid InDirId { get; set; }
    }
    public class LDir
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public DateTimeOffset DOCreate { get; set; } = DateTimeOffset.Now;
        public Guid InDirId { get; set; }
    }
    public class LFile
    {
        public Guid Id { get; set; } = Guid.Empty;
        /// <summary>
        /// The id of the first user upload this file
        /// </summary>
        public string UserId { get; set; }
        public DateTimeOffset DOUpload { get; set; } = DateTimeOffset.Now;
        /// <summary>
        /// The MD5 string, it equal the file name in specified directory
        /// </summary>
        public string MD5 { get; set; }
    }
    public class UploadFile //  ViewModel
    {
        public Guid InDirId { get; set; } = Guid.Empty;
        public Guid Token { get; set; } = Guid.NewGuid();
        public IFormFile LFormFile { get; set; }
        /// <summary>
        /// The MD5 of whole file, it'll be calculated if IsLast is true again
        /// </summary>
        public string MD5 { get; set; }
        public bool IsLast { get; set; }
        /// <summary>
        /// For updating the upload progress
        /// </summary>
        public int Progress { get; set; }
    }

    public class FileMD5
    {
        public string FileName { get; set; }
        public string MD5 { get; set; }
        public bool IsUploaded { get; set; } = false;
    }

    public class ContrastMD5
    {
        public Guid Token { get; set; }
        public Guid InDirId { get; set; }
        public IEnumerable<FileMD5> FileMD5s_ { get; set; }
    }
}
