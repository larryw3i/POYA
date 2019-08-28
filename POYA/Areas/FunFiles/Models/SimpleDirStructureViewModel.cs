

using System;
using System.Collections.Generic;

namespace POYA.Areas.FunFiles.Models
{
    public class SimpleDirStructure //  ViewModel, for copy and move selecting
    {
        public Guid Id{get;set;}
        public string Name{get;set;}
        public List<SimpleDirStructure> Items{get;set;}
    }
}