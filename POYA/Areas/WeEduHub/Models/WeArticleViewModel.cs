

using System;

namespace POYA.Areas.WeEduHub.Models
{
    public class WeArticle    //  ViewModel
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}
        public Guid SetId{get;set;}
        public string Title{get;set;}
        public string Content{get;set;}
        public DateTimeOffset  DOPublishing{get;set;}=DateTimeOffset.Now;
        public DateTimeOffset? DOModifying{get;set;}

        #region  NOTMAPPED
        #endregion
    }
    
    public class WeArticleSet
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}
        public Guid CoverFileId{get;set;}
        public string Name{get;set;}
        public string  Description{get;set;}
        public DateTimeOffset DOCreating{get;set;}=DateTimeOffset.Now;
        #region  NOTMAPPED
        #endregion
    }

    public class WeArticleFile
    {
        public Guid Id{get;set;}
        public string UserId{get;set;}
        public string SHA256HexString{get;set;}

        /// <summary>
        /// With extension
        /// </summary>
        /// <value></value>
        public string Name{get;set;}
        public DateTimeOffset DOUploading{get;set;}=DateTimeOffset.Now;
        #region  NOTMAPPED
        #endregion
    }
}