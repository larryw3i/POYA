
using System;
using POYA.Areas.FunFiles.Controllers;

namespace POYA.Areas.FunFiles.Models
{
    public class FunDir //  ViewModel
    {
        public Guid Id{get;set;}=Guid.NewGuid();
        public Guid ParentDirId{get;set;}=new FunFilesHelper().RootDirId;
        public string Name{get;set;}
        public string UserId{get;set;}
        public DateTimeOffset DOCreating{get;set;}=DateTimeOffset.Now;
    }
}