using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.EduHub.Models
{
    public class UserEArticleSet   //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public DateTimeOffset DOCreating { get; set; } = DateTimeOffset.Now;
        public string Comment{ get; set; }
    }

    public class UserEArticleHomeInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public string CoverFileMD5 { get; set; }
        public string Label { get; set; }
    }
}
