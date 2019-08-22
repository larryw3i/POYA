
using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.StaticFiles;

namespace POYA.Areas.FunFiles.Controllers
{
    public class FunFilesHelper{
        public string FunFilesRootPath(IHostingEnvironment hostingEnv) => hostingEnv.ContentRootPath + "/Areas/FunFiles/Data";
        public Guid RootDirId=Guid.Parse("a0869b67-9268-479f-a20f-4e3872afe6b9");

        /// <summary>
        /// REFERENCE   https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
        /// THANK       https://stackoverflow.com/users/23283/jaredpar
        /// </summary>
        public byte[] ConvertSHA256StringToBytes(string SHA256HexString)
        {
            return Enumerable
                .Range(0, SHA256HexString.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(SHA256HexString.Substring(x, 2), 16))
                .ToArray();
        }

        public string GetContentType(string FileNameWithExtension)
        {
            var provider = new FileExtensionContentTypeProvider();
            string contentType;
            if(!provider.TryGetContentType(FileNameWithExtension, out contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }
    }
}