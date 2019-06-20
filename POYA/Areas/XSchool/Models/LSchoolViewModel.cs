using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XSchool.Models
{
    public class LSchool    //  ViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [StringLength(maximumLength: 25, MinimumLength = 5)]
        public string Name { get; set; }
        public string MainAdminId { get; set; }
        public string CoverMD5 { get; set; }

        [StringLength(maximumLength: 100, MinimumLength = 5)]
        public string Intro { get; set; }
        public DateTimeOffset DORegistering { get; set; } = DateTimeOffset.Now;
        public bool IsVerified { get; set; } = false;

        [StringLength(maximumLength: 50, MinimumLength = 5)]
        public string Motto { get; set; }
        public string MasterUserId { get; set; }
    }

    public class LSchoolNews {
        public Guid Id { get; set; } = Guid.NewGuid();
        [StringLength(maximumLength:20,MinimumLength =5)]
        public string Title { get; set; }

        [StringLength(maximumLength: 2048, MinimumLength = 5)]
        public string Content { get; set; }
        public Guid LSchoolId { get; set; }

        public Guid PublisherId { get; set; }
        public DateTimeOffset DOPublishing { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset? DOModifying { get; set; }
    }

    public class LSchoolAdmin
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public Guid LSchoolId { get; set; }
        public DateTimeOffset DOConfirmation { get; set; } = DateTimeOffset.Now;
        public bool IsConfirmed { get; set; } = false;
        public bool IsCancelled { get; set; } = false;
    }
}
