

using System;

namespace POYA.Areas.FunFiles.Models
{
    public class FunFileByte   //  ViewModel
    {
        public Guid Id{get;set;}=Guid.NewGuid();
        public string FileSHA256HexString{get;set;}
        public string FirstUploaderId{get;set;}
        public DateTimeOffset DOUploading{get;set;}=DateTimeOffset.Now;
        
    }
}