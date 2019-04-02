using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Models
{
    public class X_doveFile //  ViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid InDirId { get; set; } = Guid.Empty;

        public string UserId { get; set; }

        [Display(Name ="File name")]
        [StringLength(maximumLength: 50)]
        public string Name { get; set; }

        [Display(Name="Content type")]
        public string ContentType { get; set; }

        [Display(Name ="Date of upload")]
        public DateTimeOffset DOUpload { get; set; } = DateTimeOffset.Now;

        [Display(Name ="Size")]
        public long Size { get; set; }   //  in byte

        public byte[] Hash { get; set; }

        public Guid CoopyOfId { get; set; }

        public byte[] FileBuffer { get; set; }
         
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }

        public bool IsLegal { get; set; } = true;

    }  

    public class ShareFoDParameter
    {
        public Guid FoDId { get; set; }
        public int Token { get; set; } = 0;
    }

    public class AvatarForm
    {
        public IFormFile AvatarImgFile { get; set; }
        public string X_DOVE_XSRF_TOKEN { get; set; }
    }

    public class Copy8Move
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ToDirId { get; set; }
        public String ReturnUrl { get; set; } = null;
        public bool IsMove { get; set; } = false;
        public bool IsFile { get; set; } = false;
        public string FoDName { get; set; } 

    }

}
