using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using POYA.Areas.XAd.Controllers;
using POYA.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace POYA.Unities.Helpers
{
    /// <summary>
    /// Application initialization
    /// </summary>
    public class AppInitialization
    {
        public string _administrator = "Administrator";
        public AppInitialization(IServiceCollection services)
        {
            var _buildServiceProvider = services.BuildServiceProvider();
            var _hostingEnv = _buildServiceProvider.GetService<IHostingEnvironment>();
            var _context = _buildServiceProvider.GetService<ApplicationDbContext>();
            var _configuration = _buildServiceProvider.GetRequiredService<IConfiguration>();
            var LFilesPath = _hostingEnv.ContentRootPath + "/Data/LFiles";
            var AvatarPath = $"{LFilesPath}/Avatars";
            var XUserFilePath = $"{LFilesPath}/XUserFile";
            var EArticleFilesPath = $"{_hostingEnv.ContentRootPath}/Areas/EduHub/Data/EArticleFiles";

            var InitialPaths = new string[] { AvatarPath, XUserFilePath, XAdCustomerHelper.XAdCustomerLicenseImgFilePath(_hostingEnv), EArticleFilesPath };
            foreach (var p in InitialPaths)
            {
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
            }
            #region INITIAL_USER_ROLE
            try
            {
                var _userRoles = new string[] { X_DOVEValues._administrator };
                var _userRoles_ = _context.Roles.Select(p => p.Name).ToListAsync().GetAwaiter().GetResult();
                _userRoles = _userRoles.Where(p => !_userRoles_.Contains(p)).ToArray();
                if (_userRoles.Count() > 0)
                {
                    foreach (var r in _userRoles)
                    {
                        _context.Roles.AddAsync(new IdentityRole { Name = r}).GetAwaiter().GetResult();
                        //  _roleManager.CreateAsync(
                    }
                }

                _context.SaveChangesAsync().GetAwaiter().GetResult();

                var _user = _context.Users.FirstOrDefaultAsync(p => p.Email == _configuration["Administration:AdminEmail"]).GetAwaiter().GetResult();
                //  FindByEmailAsync(configuration["Administration:AdminEmail"]).GetAwaiter().GetResult();
                if (_user != null)
                {
#if DEBUG
                    Console.WriteLine($"XMsg --->\tAdministrator is exist");
                    var _roleId = _context.Roles.FirstOrDefaultAsync(p => p.Name == X_DOVEValues._administrator).GetAwaiter().GetResult().Id;
#endif
                    if (_roleId != null && !_context.UserRoles.AnyAsync(p => p.UserId == _user.Id && p.RoleId == _roleId).GetAwaiter().GetResult())   //  _userManager.IsInRoleAsync(_user, _administrator).GetAwaiter().GetResult()
                    {
#if DEBUG
                        Console.WriteLine($"XMsg --->\tAdd role to administrator . . .");
#endif
                        _context.UserRoles.AddAsync(new IdentityUserRole<string> { RoleId = _roleId, UserId = _user.Id }).GetAwaiter().GetResult();
                        //  _userManager.AddToRoleAsync(_user, _administrator).GetAwaiter().GetResult();
#if DEBUG
                        Console.WriteLine($"XMsg --->\tAdd role to administrator is finish");
#endif
                    }
                }
                _context.SaveChangesAsync().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Console.WriteLine($"XMsg --->\t**You should make a migrating maybe**\n\t*{e.Message.Replace("\n","\n\t*")}");
            }

            #endregion


            #region DEPOLLUTION

            /*          
                        #region MODIFY appsettings.json
                        var _appsettingsPath=_hostingEnv.ContentRootPath+$"/appsettings.json";
                        string _output = System.IO.File.ReadAllTextAsync(_appsettingsPath).GetAwaiter().GetResult();
                        dynamic _appsettings=JsonConvert.DeserializeObject(_output);
                        _appsettings["IsInitialized"]=true;
                        _output = Newtonsoft.Json.JsonConvert.SerializeObject(_appsettings, Newtonsoft.Json.Formatting.Indented);
                         System.IO.File.WriteAllTextAsync(_appsettingsPath,_output).GetAwaiter().GetResult();
                        #endregion

                        #region CUSTOM_APP_INITIALIZATION
                        var LFilesPath = env.ContentRootPath + "/Data/LFiles";
                        var AvatarPath = $"{LFilesPath}/Avatars";
                        var XUserFilePath = $"{LFilesPath}/XUserFile";
                        var EArticleFilesPath = $"{env.ContentRootPath}/Areas/EduHub/Data/EArticleFiles";

                        var InitialPaths = new string[] { AvatarPath, XUserFilePath, XAdCustomerHelper.XAdCustomerLicenseImgFilePath(env), EArticleFilesPath };
                        foreach (var p in InitialPaths)
                        {
                            if (!Directory.Exists(p))
                            {
                                Directory.CreateDirectory(p);
                            }
                        }


                        #region INITIAL_USER_ROLE
                        var _userRoles = new string[] { _administrator };
                        //  var _userRoles_ = _context.Roles.Select(p => p.Name).ToListAsync().GetAwaiter().GetResult();
                        _userRoles = _userRoles.Where(p => !_userRoles_.Contains(p)).ToArray();
                        if (_userRoles.Count() > 0)
                        {
                            foreach (var r in _userRoles)
                            {
                                //_context.Roles.AddAsync(new IdentityRole { Name = r }).GetAwaiter().GetResult();
                                //  _roleManager.CreateAsync(
                            }
                        }
                        var _user = _context.Users.FirstOrDefaultAsync(p => p.Email == configuration["Administration:AdminEmail"]).GetAwaiter().GetResult();    
                        //  FindByEmailAsync(configuration["Administration:AdminEmail"]).GetAwaiter().GetResult();
                        if (_user != null)
                        {
            #if DEBUG
                            Console.WriteLine($"XMsg --->\tAdministrator is exist");
                            var _roleId = _context.Roles.FirstOrDefaultAsync(p=>p.Name==_administrator).GetAwaiter().GetResult().Name;
            #endif  
                            if (_roleId!=null && !_context.UserRoles.AnyAsync(p=>p.UserId==_user.Id && p.RoleId==_roleId).GetAwaiter().GetResult())   //  _userManager.IsInRoleAsync(_user, _administrator).GetAwaiter().GetResult()
                            {
            #if DEBUG
                                Console.WriteLine($"XMsg --->\tAdd role to administrator . . .");
            #endif
                                _context.UserRoles.Add(new IdentityUserRole<string> {  RoleId=_roleId, UserId=_user.Id});
                                //  _userManager.AddToRoleAsync(_user, _administrator).GetAwaiter().GetResult();
            #if DEBUG
                                Console.WriteLine($"XMsg --->\tAdd role to administrator is finish");
            #endif
                            }
                        }
                        #endregion

                        */
            #endregion

        }
    }
}
