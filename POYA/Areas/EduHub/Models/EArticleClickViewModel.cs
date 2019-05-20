using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.EduHub.Models
{
    public class EArticleClick    //  ViewModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public Guid EArticleId { get; set; }
        public string UserId { get; set; }
        public DateTimeOffset DOReading { get; set; } = DateTimeOffset.Now;
    }
    public class EArticleComment
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid EArticleId { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset DOComment { get; set; } = DateTimeOffset.Now;
        public bool IsLegal { get; set; } = true;
    }
}
