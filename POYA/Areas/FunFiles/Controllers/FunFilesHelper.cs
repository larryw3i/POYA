
using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace POYA.Areas.FunFiles.Controllers
{
    public class FunFilesHelper
    {

        public string SHA256BytesToHexString(byte[] SHA256)=> BitConverter.ToString(SHA256).Replace("-","");

        /// <summary>
        /// Optime the storage size for showing
        /// </summary>
        /// <param name="_byte">The byte</param>
        /// <returns>Return the optimed string</returns>
        public string OptimizeFileSize(long _byte) =>
            _byte < 1024 ? $"{_byte}b" :
            _byte < Math.Pow(1024, 2) ? $"{(_byte / 1024).ToString("0.0")}K" :
            _byte < Math.Pow(1024, 3) ? $"{(_byte / Math.Pow(1024, 2)).ToString("0.0")}M" :
            $"{(_byte / Math.Pow(1024, 3)).ToString("0.0")}G";

        /// <summary>
        /// hostingEnv.ContentRootPath + "/Areas/FunFiles/Data"
        /// </summary>
        /// <param name="hostingEnv"></param>
        /// <returns></returns>
        public string FunFilesRootPath(IHostingEnvironment hostingEnv) => hostingEnv.ContentRootPath + "/Areas/FunFiles/Data";
        public Guid RootDirId=Guid.Parse("a0869b67-9268-479f-a20f-4e3872afe6b9");

        public byte[] GetFormFileBytes(IFormFile formFile){

            var MemoryStream_ = new MemoryStream();
            
            formFile.CopyToAsync(MemoryStream_)
                .GetAwaiter()
                .GetResult();

            return MemoryStream_.ToArray();
        }

        /// <summary>
        /// REFERENCE   https://stackoverflow.com/questions/321370/how-can-i-convert-a-hex-string-to-a-byte-array
        /// THANK       https://stackoverflow.com/users/23283/jaredpar
        /// </summary>
        public byte[] SHA256HexStringToBytes(string SHA256HexString)
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