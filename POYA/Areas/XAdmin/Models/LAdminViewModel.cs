using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XAdmin.Models
{
    /// <summary>
    /// A role model
    /// </summary>
    public class LAdmin //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        [Display(Name ="Role name")]
        public string RoleName { get; set; }
        [Display(Name = "Date of submitting information")]
        public DateTimeOffset DOSubmit { get; set; } = DateTimeOffset.Now;
        [Display(Name = "Date of confirming information")]
        public DateTimeOffset? DOConfirmation { get; set; }
        [Display(Name ="Is information confirmed")]
        public bool IsInfoConfirmed { get; set; } = false;
        [Display(Name = "The -related files- you got from the administrator")]
        public List<IFormFile> RelatedFiles { get; set; }

    }
}
