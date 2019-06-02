using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POYA.Areas.XUserFile.Models;
using POYA.Data;
using POYA.Unities.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace POYA.Areas.XUserFile.Controllers
{
    public class XUserFileHelper
    {
        public async Task<string> LWriteBufferToFileAsync( IHostingEnvironment _hostingEnv , IFormFile formFile)
        {
            var MemoryStream_ = new MemoryStream();
            await formFile.CopyToAsync(MemoryStream_);
            var FileBytes = MemoryStream_.ToArray();
            var MD5_ = GetFileMD5(FileBytes);
            var UploadFileMD5s = System.IO.Directory.GetFiles(X_DOVEValues.FileStoragePath(_hostingEnv))
             .Select(p => System.IO.Path.GetFileNameWithoutExtension(p)).ToList();

            if (UploadFileMD5s.Contains(MD5_))
            {
                return MD5_;
            }

            var FilePath = X_DOVEValues.FileStoragePath(_hostingEnv) + MD5_;
            //  System.IO.File.Create(FilePath);
            await System.IO.File.WriteAllBytesAsync(FilePath, FileBytes);
            return MD5_;

        }
        /// <summary>
        /// determine the MD5 in IEnumerableLMD5 is match the MD5 of uploaded files or not
        /// </summary>
        /// <param name="env">The IHostingEnvironment for getting FileStoragePath</param>
        /// <param name="lMD5s">The IEnumerableLMD5</param>
        /// <returns></returns>
        public List<LMD5> LCheckMD5(IHostingEnvironment env,List<LMD5> lMD5s)
        { 
            
            var UploadFileMD5s =System.IO.Directory.GetFiles(X_DOVEValues.FileStoragePath(env))
                .Select(p => System.IO.Path.GetFileNameWithoutExtension(p)).ToList();

            foreach (var m in lMD5s)
                if (UploadFileMD5s.Contains(m.FileMD5))
                    m.IsUploaded = true;

            return lMD5s;
        }

        /// <summary>
        /// Get the md5 of file byte array
        /// </summary>
        /// <param name="FileBytes">
        /// File byte array
        /// </param>
        /// <returns></returns>
        public string GetFileMD5(byte[] FileBytes)
        {
            var Md5_ = MD5.Create();
            var MD5Bytes = Md5_.ComputeHash(FileBytes);
            var sb = new StringBuilder();
            for (int i = 0; i < MD5Bytes.Length; i++)
            {
                sb.Append(MD5Bytes[i].ToString("x2"));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Determine a id of <see cref="LUserFile"/> or <see cref="LDir"/> is included in a directory
        /// </summary>
        /// <returns>
        /// Return <see cref="true"/> if the id is included in DirId, or <see cref="false"/>
        /// </returns>
        public bool IsFileOrDirInDir(IEnumerable<ID8InDirId> iD8InDirIds, Guid id, Guid DirId)
        {
            var _InDirId = iD8InDirIds.Where(p => p.Id == id).Select(p => p.InDirId).FirstOrDefault();
            if (_InDirId == null || _InDirId == Guid.Empty) return false;
            while (_InDirId != Guid.Empty)
            {
                if (_InDirId == DirId)
                {
                    return true;
                }
                //  _InDirId = ID8InDirIds.Where(p => p.Id == _InDirId).Select(p => p.InDirId).FirstOrDefault();
                _InDirId = iD8InDirIds.Where(p => p.Id == _InDirId).Select(p => p.InDirId).FirstOrDefault();
            }
            return false;
        }

        /// <summary>
        /// Get all subdirectories in a directory
        /// </summary>
        /// <returns>List<LDir></returns>
        public List<LDir> GetAllSubDirs(List<LDir> lDirs, Guid DirId)
        {
            var SubDirs = new List<LDir>();
            var _iD8InDirIds = lDirs.Select(p => new ID8InDirId { InDirId = p.InDirId, Id = p.Id }).ToList();
            foreach (var i in lDirs)
            {
                if (IsFileOrDirInDir(_iD8InDirIds, i.Id, DirId))
                {
                    SubDirs.Add(i);
                }
            }
            return SubDirs;
        }

        public List<LUserFile> GetAllSubFiles(List<LDir> lSubDirs, List<LUserFile> lUserFiles, Guid DirId)
        {
            var _DirIDs = lSubDirs.Select(p => p.Id).ToList();
            _DirIDs.Add(DirId);
            return lUserFiles.Where(o => _DirIDs.Contains(o.InDirId)).ToList();
        }
        #region
        /*
        public List<LSharingDirMap> MakeSubFDCopy(List<LDir> lDirs, List<LUserFile> lUserFiles, Guid InDirId)
        {
            var _InDir = lDirs.Where(p => p.Id == InDirId).FirstOrDefault();
            if (_InDir == null)
            {
                return null;
            }
            var _AllSubDs = GetAllSubDirs(lDirs, InDirId);
            var _AllSubFs = GetAllSubFiles(_AllSubDs, lUserFiles, InDirId);
            var _LSharingDirMap = _AllSubDs.Select(p => new LSharingDirMap
            {
                OrginalId = p.Id,
                OrginalInDirId = p.InDirId
            }).Union(_AllSubFs.Select(p => new LSharingDirMap
            {
                OrginalInDirId = p.InDirId,
                OrginalId = p.Id
            })).ToList();
            return _LSharingDirMap;
        }
        */
        #endregion
    }
}
