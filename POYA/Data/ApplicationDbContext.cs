using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POYA.Models;
using POYA.Areas.EduHub.Models;
using POYA.Areas.XUserFile.Models;
using POYA.Areas.Identity;
using POYA.Areas.XAd.Models;
using POYA.Areas.FunAdmin.Models;
using POYA.Areas.FunFiles.Models;
using POYA.Areas.iEduHub.Models;
namespace POYA.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<POYA.Models.X_doveUserInfo> X_DoveUserInfos { get; set; }
        public DbSet<POYA.Areas.XUserFile.Models.LUserFile> LUserFile { get; set; }
        public DbSet<POYA.Areas.XUserFile.Models.LFile> LFile { get; set; }
        public DbSet<POYA.Areas.XUserFile.Models.LDir> LDir { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EArticle> EArticle { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EArticleUserReadRecord> EArticleUserReadRecords { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EArticleFile> EArticleFiles { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.UserEArticleSet> UserEArticleSet { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.UserEArticleHomeInfo> UserEArticleHomeInfos { get; set; }
        public DbSet<POYA.Areas.XAd.Models.XAdCustomer> XAdCustomer { get; set; }
        public DbSet<POYA.Areas.XAd.Models.XAdCustomerLicense> XAdCustomerLicenses { get; set; }
        public DbSet<POYA.Areas.XAd.Models.LAd> LAds{get;set;}
        public DbSet<POYA.Areas.XAd.Models.LAdImage> LAdImages{get;set;}
        public DbSet<POYA.Areas.XAd.Models.UserFeedback> UserFeedbacks{get;set;}
        public DbSet<POYA.Areas.XAd.Models.UserFeedbackImage> UserFeedbackImages{get;set;}
        public DbSet<POYA.Areas.XAd.Models.UserPraiseOr> UserPraiseOrs{get;set;}
        public DbSet<POYA.Areas.FunAdmin.Models.FContentCheck> FContentCheck { get; set; }
        public DbSet<POYA.Areas.FunFiles.Models.FunDir> FunDir { get; set; }
        public DbSet<POYA.Areas.FunFiles.Models.FunFileByte> FunFileByte { get; set; }
        public DbSet<POYA.Areas.FunFiles.Models.FunYourFile> FunYourFile { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EduHubFile> EduHubFiles{get;set;}
        public DbSet<POYA.Areas.iEduHub.Models.WeArticle> WeArticle { get; set; }
        public DbSet<POYA.Areas.iEduHub.Models.WeArticleSet> WeArticleSet { get; set; }
        public DbSet<POYA.Areas.iEduHub.Models.WeArticleFile> WeArticleFile { get; set; }
        
    }
}
