
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.FunFiles.Models;
using POYA.Data;

namespace POYA.Areas.FunFiles.Controllers
{
    public class FunFilesHelper
    {

        public bool IsIdInParentDirId(Guid ParentDirId, Guid Id, List<IdAndParentId> IdAndParentIds)
        {   
            var _ParentDirId=IdAndParentIds
                .Where(p=>p.Id==Id)
                .Select(p=>p.ParentId)
                .FirstOrDefault();

            var _IsIdInParentDirId=_ParentDirId==ParentDirId;
            
            while(!_IsIdInParentDirId && _ParentDirId!=RootDirId)
            {
                _ParentDirId=IdAndParentIds.Where(p=>p.Id==_ParentDirId).Select(p=>p.ParentId).FirstOrDefault();
                _ParentDirId=_ParentDirId==Guid.Empty?RootDirId:_ParentDirId;
                _IsIdInParentDirId= _ParentDirId==ParentDirId;
            }
            
            return _IsIdInParentDirId;
        }

        /// <summary>
        /// _FunDirs.OrderBy(p=>p.DOCreating).ToList()
        /// </summary>
        /// <param name="ParentDirId"></param>
        /// <param name="_UserId"></param>
        /// <param name="_context"></param>
        /// <returns></returns>
        public List<FunDir> GetPathFunDir(Guid? ParentDirId, string _UserId, List<FunDir> FunDirs)
        {
            var _ParentDirId = ParentDirId ?? RootDirId;

            var _FunDirs = new List<FunDir>(){
                new FunDir{
                    DOCreating=DateTimeOffset.MinValue,
                    Id=RootDirId,
                    Name="root",
                    ParentDirId=RootDirId,
                    UserId=_UserId
                }
            };

            while (_ParentDirId != RootDirId)
            {
                var _FunDir = FunDirs.FirstOrDefault(p => p.Id == _ParentDirId);
                if (_FunDir == null)
                {
                    break;
                }
                _FunDirs.Add(_FunDir);
                _ParentDirId = _FunDir.ParentDirId;
            }

            return _FunDirs.OrderBy(p => p.DOCreating).ToList();
        }


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

        /// <summary>
        /// a0869b67-9268-479f-a20f-4e3872afe6b9
        /// </summary>
        /// <returns></returns>
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