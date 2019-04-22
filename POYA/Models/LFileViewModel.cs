using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Models
{
    public class LUserFile  //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string MD5 { get; set; } = string.Empty;
        public string UserId { get; set; }
        /// <summary>
        /// It is shared if SharedCode isn't Guid.Empty
        /// </summary>
        public Guid SharedCode { get; set; } = Guid.Empty;
        /// <summary>
        /// The default value is DateTimeOffset.Now
        /// </summary>
        [Display(Name="Date")]
        public DateTimeOffset DOGenerating { get; set; } = DateTimeOffset.Now;
        [Display(Name="Name")]
        public string Name { get; set; }
        /// <summary>
        /// The defaut value is Guid.Empty
        /// </summary>
        public Guid InDirId { get; set; } = Guid.Empty;
        public string ContentType { get; set; }
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public string InDirName { get; set; }
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public IFormFile LFormFile { get; set; }
    }
    public class LDir
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Display(Name="Directory")]
        public string Name { get; set; }
        /// <summary>
        /// The default value is DateTimeOffset.Now
        /// </summary>
        [Display(Name ="Date")]
        public DateTimeOffset DOCreate { get; set; } = DateTimeOffset.Now;
        /// <summary>
        /// The default value is Guid.Empty
        /// </summary>
        public Guid InDirId { get; set; } = Guid.Empty;
        public string UserId { get; set; }
        [NotMapped]
        public string InDirName { get; set; }
        [NotMapped]
        public string ReturnUrl { get; set; } = null;
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

    //<<<<<<<<<<<<<<<<<<<<<<<<<<<SEPARATOR>>>>>>>>>>>>>>>>>>>//
    public class MediaType
    {
        //  Name,Template,Reference
        public string Name { get; set; }
        public string Template { get; set; }
        public string Reference { get; set; }
    }
    public class ContrastMD5
    {
        public Guid InDirId { get; set; } = Guid.Empty;
        public IEnumerable<File8MD5> File8MD5s { get; set; }
    }

    public class File8MD5
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string MD5 { get; set; }
    }

    public class LFilePost
    {
        /// <summary>
        /// The id for callback
        /// </summary>
        public int Id { get; set; }
        public IFormFile _LFile { get; set; }
        public Guid InDirId { get; set; }
    }
}
