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
        /// <summary>
        /// It is the new id of shared file or directory
        /// </summary>
        [Display(Name = "Shared ID")]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid OrginalId { get; set; }
        /// <summary>
        /// Some notice should been said
        /// </summary>
        public string Comment { get; set; }

    }

}
