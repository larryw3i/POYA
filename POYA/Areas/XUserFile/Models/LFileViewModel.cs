using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XUserFile.Models
{
    public class LUserFile : FileDirCommon  //  ViewModel
    {
        public string SHA256 { get; set; } = string.Empty;

        #region 

        /// <summary>
        /// The default value is DateTimeOffset.Now
        /// </summary>
        #endregion
        [Display(Name = "Date")]
        public DateTimeOffset DOCreate { get; set; } = DateTimeOffset.Now;

        public bool IsEArticleFile { get; set; } = false;

        public bool IsLegal { get; set; } = true;

        #region DEPOLLUTION

        #region SIZE

        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public long Size { get; set; }

        #region 
        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        #endregion
        public string OptimizedSize { get; set; }
        #endregion

        #region 

        /// <summary>
        /// [NotMapped]
        /// </summary>
        #endregion
        [NotMapped]
        public string ContentType { get; set; }

        #region
        /// <summary>
        /// [NotMapped]
        /// </summary>

        #endregion
        [NotMapped]
        public IFormFile LFormFile { get; set; }
        #endregion
    }

    public class LDir : FileDirCommon
    {
        /// <summary>
        /// The default value is DateTimeOffset.Now
        /// </summary>
        [Display(Name = "Date")]
        public DateTimeOffset DOCreate { get; set; } = DateTimeOffset.Now;

    }


    public class LFile
    {
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// The id of the first user upload this file
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The default is DateTimeOffset.Now
        /// </summary>
        public DateTimeOffset DOUpload { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// The SHA256 string, it equal the file name in specified directory
        /// </summary>
        public string SHA256 { get; set; }
    }

    #region DEPOLLUTION


    public class ID8InDirId
    {
        public Guid Id { get; set; }
        public Guid InDirId { get; set; }
    }

    public class ID8NewID
    {
        public Guid NewId { get; set; }
        //  public Guid NewInDirId { get; set; }
        public Guid OriginalId { get; set; }
        public Guid OriginalInDirId { get; set; }
    }


    public class FileDirCommon
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; }

        /// <summary>
        /// The default value is Guid.Empty
        /// </summary>
        public Guid InDirId { get; set; } = Guid.Empty;

        [Display(Name = "Name")]
        public string Name { get; set; }



        #region DEPOLLUTION

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public string InDirName { get; set; }

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public string InFullPath { get; set; } = "root";

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public List<SelectListItem> UserAllSubDirSelectListItems { get; set; }

        /// <summary>
        /// [NotMapped]
        /// Determine is coping(is 1) file to directory or moving(is 2), do nothing if it is 0
        /// </summary>
        [NotMapped]
        public CopyMove CopyMove { get; set; } = CopyMove.DoNoThing;

        /// <summary>
        /// [NotMapped]
        /// </summary>
        [NotMapped]
        public List<SelectListItem> CopyMoveSelectListItems { get; set; }

        #endregion

    }

    public enum CopyMove
    {
        Copy = 1,
        Move = 2,
        DoNoThing = 0
    };

    public class MediaType
    {
        //  Name,Template,Reference
        public string Name { get; set; }
        public string Template { get; set; }
        //public string Reference { get; set; }
    }

    public class ContrastSHA256
    {
        public Guid InDirId { get; set; } = Guid.Empty;
        public IEnumerable<File8SHA256> File8SHA256s { get; set; }
    }

    public class File8SHA256
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string SHA256 { get; set; }
    }

    public class LFilePost
    {
        #region 

        /// <summary>
        /// The id for callback
        /// </summary>
        #endregion
        public string Id { get; set; }
        public IFormFile LFile { get; set; }
        public Guid InDirId { get; set; }
    }

    /// <summary>
    /// For LCheckSHA256
    /// </summary>
    public class LSHA256
    {
        public string FileSHA256 { get; set; }
        /// <summary>
        /// The default is <see langword="false"/>
        /// </summary>
        public bool IsUploaded { get; set; } = false;
    }
    #endregion

}


