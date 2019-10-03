using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using POYA.Models;
using POYA.Areas.FunAdmin.Models;
using POYA.Areas.FunFiles.Models;
using POYA.Areas.WeEduHub.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace POYA.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<POYA.Models.X_doveUserInfo> X_DoveUserInfos { get; set; }
        public DbSet<POYA.Areas.WeEduHub.Models.FContentCheck> FContentCheck { get; set; }
        public DbSet<POYA.Areas.FunFiles.Models.FunDir> FunDir { get; set; }
        public DbSet<POYA.Areas.FunFiles.Models.FunFileByte> FunFileByte { get; set; }
        public DbSet<POYA.Areas.FunFiles.Models.FunYourFile> FunYourFile { get; set; }
        public DbSet<POYA.Areas.WeEduHub.Models.WeArticle> WeArticle { get; set; }
        public DbSet<POYA.Areas.WeEduHub.Models.WeArticleSet> WeArticleSet { get; set; }
        public DbSet<POYA.Areas.WeEduHub.Models.WeArticleFile> WeArticleFile { get; set; }
        public DbSet<POYA.Areas.WeEduHub.Models.FunComment> FunComment { get; set; }

    }
}
