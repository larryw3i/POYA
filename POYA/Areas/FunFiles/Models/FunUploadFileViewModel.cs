
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace POYA.Areas.FunFiles.Models
{
    public class FunUploadFile  //  ViewModel
    {
        public Guid ParentDirId{get;set;}
        public List<IFormFile> FunFiles{get;set;} 
        
    }
    

}