using Microsoft.EntityFrameworkCore;
using POYA.Areas.XUserFile.Models;
using POYA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Areas.XUserFile.Controllers
{
    public class XUserFileHelper
    {
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
    }
}
