
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using POYA.Areas.FunFiles.Controllers;
using POYA.Areas.XUserFile.Models;
using POYA.Unities.Helpers;

namespace POYA.Areas.EduHub.Controllers
{
    public class EduHubHelper
    {
        public string CancelSearchKeyCmd = "E58AE815-0CE2-469A-BD46-3C68B99547D9";

        /// <summary>
        /// Get the MD5 of file byte array
        /// </summary>
        /// <param name="FileBytes">
        /// File byte array
        /// </param>
        /// <returns></returns>
        public string GetFileSHA256(byte[] FileBytes)
        {
            var SHA256Bytes = SHA256.Create().ComputeHash(FileBytes);
            
            return new FunFilesHelper().SHA256BytesToHexString(SHA256Bytes);
        }
         
         
        /// <summary>
        /// determine the MD5 in IEnumerableLMD5 is match the MD5 of uploaded files or not
        /// </summary>
        /// <param name="env">The IHostingEnvironment for getting FileStoragePath</param>
        /// <param name="lMD5s">The IEnumerableLMD5</param>
        /// <returns></returns>
        public List<LSHA256> LCheckSHA256(IHostingEnvironment env, List<LSHA256> lSHA256s)
        {

            var UploadFileSHA256s = System.IO.Directory.GetFiles(X_DOVEValues.FileStoragePath(env))
                .Select(p => System.IO.Path.GetFileNameWithoutExtension(p)).ToList();

            foreach (var m in lSHA256s)
                if (UploadFileSHA256s.Contains(m.FileSHA256))
                    m.IsUploaded = true;

            return lSHA256s;
        }
    }
}