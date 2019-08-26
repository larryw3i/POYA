
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace POYA.Areas.FunFiles.Models
{
    public class SHA256Compare
    {
        public Guid ParentDirId{get;set;}
        public List<FileSHA256> FileSHA256s{get;set;}
    }

    public class FileSHA256
    {
        public string Id{get;set;}
        public string Name{get;set;}
        public string SHA256HexString{get;set;}
    }
}