

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using POYA.Areas.WeEduHub.Models;

namespace POYA.Areas.WeEduHub.Controllers
{
    public class WeEduHubArticleClassHelper
    {
        private readonly IWebHostEnvironment _hostingEnv;
        private readonly Regex _unicode2StringRegex=new Regex(@"\\u([0-9A-F]{4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        public WeEduHubArticleClassHelper(
           IWebHostEnvironment hostingEnv
        ){
            _hostingEnv=hostingEnv;
        }


        /// <summary>
        /// /// FROM        https://blog.csdn.net/qq_26422355/article/details/82716824
        /// THANK       https://blog.csdn.net/qq_26422355
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string Unicode2String(string source)=>_unicode2StringRegex.Replace(source, x => string.Empty + Convert.ToChar(Convert.ToUInt16(x.Result("$1"), 16)));
        

        public string WeArticleClassCsvPath()=>_hostingEnv.WebRootPath+"/app_data/wearticle_class.csv";

        public void InitialWeArticleClassName(ref WeArticle weArticle,Guid ClassId)
        {
            var reader = new StreamReader(WeArticleClassCsvPath());
            var csv = new CsvReader(reader);
            var _Records= csv.GetRecords<WeArticleFirstClass>().ToList();
            var _SecondClass=_Records.Where(p=>p.Id==ClassId).FirstOrDefault();
            weArticle.SecondClassName=_SecondClass.Name;
            weArticle.FirstClassName=_Records.Where(p=>p.Code==_SecondClass.Code.Substring(0,3)).Select(p=>p.Name).FirstOrDefault();
        }


        public Guid GetFirstClassIdBySecondClassId(Guid Id)
        {
            var reader = new StreamReader(WeArticleClassCsvPath());
            var csv = new CsvReader(reader);
            var _Records= csv.GetRecords<WeArticleFirstClass>().ToList();
            var _SecondClassCode=_Records.Where(p=>p.Id==Id).Select(p=>p.Code).FirstOrDefault();
            return _Records.Where(p=>p.Code==_SecondClassCode.Substring(0,3)).Select(p=>p.Id).FirstOrDefault();
        }

        public string GetClassCodeByClassId(Guid Id)
        {
            var reader = new StreamReader(WeArticleClassCsvPath());
            var csv = new CsvReader(reader);
            return csv.GetRecords<WeArticleFirstClass>()
                .Where(p=>p.Id==Id)
                .Select(p=>p.Code)
                .FirstOrDefault();
        }

        public List<WeArticleFirstClass> GetAllFirstClasses()
        {
            
            var reader = new StreamReader(WeArticleClassCsvPath());
            var csv = new CsvReader(reader);
            return csv.GetRecords<WeArticleFirstClass>()
                .Where(p=>p.Code.Length==3)
                .ToList();
        }
        
        public List<WeArticleSecondClass> GetAllSecondClasses()
        {
            
            var reader = new StreamReader(WeArticleClassCsvPath());
            var csv = new CsvReader(reader);
            return csv.GetRecords<WeArticleSecondClass>()
                .Where(p=>p.Code.Length==5)
                .ToList();
        }


        public List<WeArticleSecondClass> GetSecondClassesByFirstClassCode(string FirstCode)
        {
            
            var reader = new StreamReader(WeArticleClassCsvPath());
            var csv = new CsvReader(reader);
            return csv.GetRecords<WeArticleSecondClass>()
                .Where(p=>p.Code.StartsWith(FirstCode) && p.Code.Length==5)
                .ToList();
        }

        public WeArticleFirstClass GetFirstClassBySecondClassCode(string SecondCode)
        {
            
            var reader = new StreamReader(WeArticleClassCsvPath());
            var csv = new CsvReader(reader);
            return csv.GetRecords<WeArticleFirstClass>()
                .Where(p=>p.Code==SecondCode.Substring(0,3))
                .ToList().FirstOrDefault();
        }
    }
}