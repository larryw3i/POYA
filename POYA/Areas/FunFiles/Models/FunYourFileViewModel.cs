
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using POYA.Areas.FunFiles.Controllers;

namespace POYA.Areas.FunFiles.Models
{
    public class FunYourFile   //  ViewModel
    {
        public Guid Id{get;set;}=Guid.NewGuid();
        
        public Guid FileByteId{get;set;}

        public Guid ParentDirId{get;set;}=new FunFilesHelper().RootDirId;

        public string UserId{get;set;}

        /// <summary>
        /// With extension
        /// </summary>
        /// <value></value>
        [StringLength(maximumLength:100,MinimumLength=1)]
        public string Name{get;set;}

        public DateTimeOffset DOUploading{get;set;}=DateTimeOffset.Now;

        #region  NotMapped
        
        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public string FileSize{get;set;}=string.Empty;

        /// <summary>
        /// NotMapped
        /// </summary>
        /// <value></value>
        [NotMapped]
        public List<FunDir> FilePathFunDirs{get;set;}

        #endregion

    }
}