using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POYA.Models;
using POYA.Areas.EduHub.Models;
using POYA.Areas.XUserFile.Models;
using POYA.Areas.XLaw.Models;
using POYA.Areas.Identity;
using POYA.Areas.XAd.Models;
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
        public DbSet<POYA.Areas.XLaw.Models.Complaint> Complaint { get; set; }
        public DbSet<POYA.Areas.XLaw.Models.Arbitrament> Arbitrament { get; set; }
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
        
        #region 

        /*        
        public DbSet<POYA.Areas.Identity.POYALRole> POYALRole { get; set; }


        public DbSet<POYA.Areas.XSchool.Models.LSchool> LSchool { get; set; }
        public DbSet<POYA.Areas.XSchool.Models.LSchoolNews> LSchoolNews { get; set; }
        public DbSet<POYA.Areas.XSchool.Models.LSchoolAdmin> LSchoolAdmin { get; set; }
        public DbSet<POYA.Areas.XSchool.Models.LSchoolAffiliation> LSchoolAffiliation { get; set; }
        public DbSet<POYA.Areas.Identity.POYALRole> POYALRole { get; set; }

        public DbSet<POYA.Areas.DeveloperZone.Models.XDeveloper> XDeveloper { get; set; }
        public DbSet<POYA.Areas.DeveloperZone.Models.XDeveloperNote> XDeveloperNote { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EArticleComment> EArticleComment { get; set; }
        #region EARTICLE_CLASS
        public DbSet<LField> LFields { get; set; }
        public DbSet<LAdvancedClass> LAdvancedClasses { get; set; }
        public DbSet<LSecondaryClass> LSecondaryClasses { get; set; }
        public DbSet<LGrade> LGrades { get; set; }
        public DbSet<LGradeComment> LGradeComments { get; set; }
        #endregion

        public DbSet<POYA.Areas.EduHub.Models.EGrade> EGrade { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.ESubject> ESubject { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EType> EType { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EVideo> EVideo { get; set; }
        public DbSet<POYA.Areas.XUserFile.Models.LSharing> LSharings { get; set; }
        //  public DbSet<POYA.Areas.XUserFile.Models.LSharingDirMap> LSharedDirMaps { get; set; }
        public DbSet<POYA.Areas.XUserFile.Models.LUserSharingFile> LUserSharingFile { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.Video> Video { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.Type> Type { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EduHubArticle> EduHubArticle { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EduHubGrade> EduHubGrade { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EduHubSubject> EduHubSubject { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EduHubType> EduHubType { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EduHubVideo> EduHubVideo { get; set; }
        public DbSet<POYA.Models.EduHubArticle> EduHubArticle { get; set; }
        public DbSet<POYA.Models.EduHubVideo> EduHubVideo { get; set; }
        public DbSet<POYA.Models.EduHubSubject> EduHubSubject { get; set; }
        public DbSet<POYA.Models.EduHubType> EduHubType { get; set; }
        public DbSet<POYA.Models.EduHubGrade> EduHubGrade { get; set; }
        public DbSet<POYA.Models.X_doveUserFileTag> X_doveUserFileTag { get; set; }
        public DbSet<POYA.Models.X_doveFile>  X_DoveFiles { get; set; }
        public DbSet<POYA.Models.X_doveDirectory> X_doveDirectories { get; set; }
        public DbSet<POYA.Models.X_doveFile> X_doveFiles { get; set; }
        public DbSet<POYA.Models.SharedF> SharedFs { get; set; }
        public DbSet<POYA.Models.SharedD> SharedDs { get; set; }
        public DbSet<POYA.Models.Copy8Move> Copy8MoveFile { get; set; }
        */

        #endregion
    }
}
