

using System;

namespace POYA.Areas.EduHub.Models
{
    public class EduHubFile
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
        /// The MD5 string, it equal the file name in specified directory
        /// </summary>
        public string SHA256 { get; set; }
    }
}