using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Models
{
    public class X_doveDirectory    //  ViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string UserId { get; set; }

        [Display(Name ="Directory Name")]
        [StringLength(maximumLength:50,MinimumLength =1)]
        public string Name { get; set; } 

        public Guid InDirId { get; set; } = Guid.Empty;

        [Display(Name="Date of create")]
        public DateTimeOffset DOCreate { get; set; } = DateTimeOffset.Now;
         
    }
}
