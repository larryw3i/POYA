using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace POYA
{
    public class Program
    {
        
        /**
         * DOT1     FINISH ONE THING WELL BEFORE YOU DO ANOTHER
         */

        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            //  .UseUrls("http://*:3456")
            //  .UseKestrel()
            .UseStartup<Startup>();
    }
}
