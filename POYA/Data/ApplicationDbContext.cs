using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using POYA.Models;
using POYA.Areas.EduHub.Models;
namespace POYA.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<POYA.Models.X_doveUserInfo> X_DoveUserInfos { get; set; }
        public DbSet<POYA.Models.LUserFile> LUserFile { get; set; }
        public DbSet<POYA.Models.LFile> LFile { get; set; }
        public DbSet<POYA.Models.LDir> LDir { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EArticle> EArticle { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EGrade> EGrade { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.ESubject> ESubject { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EType> EType { get; set; }
        public DbSet<POYA.Areas.EduHub.Models.EVideo> EVideo { get; set; }
        /*
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
    }
}
