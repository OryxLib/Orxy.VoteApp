using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Oryx.VoteApp.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseUrls("http://localhost:5000")
                .UseStartup<Startup>()
                .UseLibuv(options =>
                {
                    options.ThreadCount = Environment.ProcessorCount;
                })
                .UseHttpSys(options =>
                {
                    options.MaxAccepts = 65535;
                    options.MaxConnections = -1;
                    options.RequestQueueLimit = 65535;
                })
                .UseKestrel(options =>
                {
                    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(30);
                    options.Limits.MaxConcurrentConnections = null;
                    options.Limits.MaxConcurrentUpgradedConnections = null;
                    options.Limits.MaxRequestBodySize = null;
                    options.Limits.MaxRequestBufferSize = null;
                    options.Limits.MaxRequestLineSize = int.MaxValue; 
                    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30); 
                }) 
                .Build();
    }
}
