

using System;

namespace POYA.Areas.FunFiles.Models
{
    public class FunFileByte   //  ViewModel
    {
        public Guid Id{get;set;}=Guid.NewGuid();
        public byte[] FileSHA256{get;set;}
        public string FirstUploaderId{get;set;}
        public DateTimeOffset DOUploading{get;set;}=DateTimeOffset.Now;
        
    }
}