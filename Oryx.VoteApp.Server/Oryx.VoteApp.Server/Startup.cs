using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using NLog.Extensions.Logging;
using Oryx.MaxConcurrentRequests.Middlewares;
using Oryx.VoteApp.Server.Models;
using Oryx.VoteApp.Server.PostManager;
using Oryx.VoteApp.Server.Ultility;
using Oryx.VoteApp.Server.WebSocketHandler;
using Oryx.WebSocket;
using Oryx.WebSocket.Extension;
using Oryx.WebSocket.Extension.Builder;
using Oryx.WebSocket.Extension.DependencyInjection;
using Oryx.Wx.Core;
using Oryx.Wx.MiniApp;
using Oryx.VoteApp.Server.DbConextPool;
using RdKafka;
using System.Threading;
using Oryx.VoteApp.Server.Services;
using StackExchange.Redis;
using Microsoft.AspNetCore.DataProtection;
using Oryx.VoteApp.Server.ApplicationBuilderExtension;

namespace Oryx.VoteApp.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddMvc().AddSessionStateTempDataProvider();
            services.AddSession(opts =>
            {
                opts.IdleTimeout = TimeSpan.FromDays(1);
            });
            //services.AddDbContextPool<VoteAppDbContext>((serviceProvider, option) =>
            //{
            //    serviceProvider.
            //});

            services.AddDbContextPool<VoteAppDbContext>(optBuilder =>
            { optBuilder.UseMySql("server=101.132.130.133;database=VoteApp;user=root;password=Linengneng123#;Character Set=utf8;", opts =>
                   {
                         opts.CommandTimeout(30000);
                         opts.EnableRetryOnFailure(3);
                         opts.MaxBatchSize(1000);
                     })
                     ;
            });

            var singleDbOption = new DbContextOptions<SingletonAppDbContext>();
            services.AddSingleton<SingletonAppDbContext>(new SingletonAppDbContext(singleDbOption));

            services.AddSingleton<IConfiguration>(Configuration);
            services.AddSingleton<QiniuTool>();
            //services.AddSingleton<DbContextPool>();
            //services.AddSingleton<IOryxWebSocketPool, OryxWebSocketPool>();
            //services.AddSingleton<IOryxWebSocket, OryxWebSocket>();
            services.AddOryxWebSocket();
            services.AddSingleton<TestHandler>();
            services.AddSingleton<DbOperationQueue>();
            services.AddSingleton<GlobalVoteResultDic>();
            services.AddSingleton<DbOperationMngr>();
            services.AddDistributedMemoryCache();
            services.AddDistributedRedisCache(option =>
            {
                option.Configuration = "101.132.130.133:6379,password=Linengneng123#";
                option.InstanceName = "master";
            });
            var redis = ConnectionMultiplexer.Connect("101.132.130.133:6379,password=Linengneng123#");
            services.AddDataProtection()
                .SetApplicationName("session_application_name") 
                .PersistKeysToRedis(redis, "DataProtection-Keys");
            services.AddSingleton<WxAccessToken>(new WxAccessToken(Configuration["AppId"], Configuration["Secret"]));
            services.AddSingleton<WxaCode>();
            services.AddSingleton<RabbitMQClient>();
            //services.AddSingleton<Producer>(new Producer("kafka-cn-hangzhou-share001.aliyun.com:8080"));
            //services.AddSingleton<Topic>(services.BuildServiceProvider().GetRequiredService<Producer>().Topic("alikafka-votePost"));
            var builder = new ContainerBuilder();
            builder.Populate(services);
            var container = builder.Build();
            return container.Resolve<IServiceProvider>();
        }
        //PM> install-package MySql.Data.EntityFrameworkCore
        //install-package MySql.Data.EntityFrameworkCore.Design

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }
            NLog.LogManager.LoadConfiguration("nlog.config");

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            loggerFactory.AddNLog();
            app.UseRabbitMQClient();
            app.UseSession();
            app.UseStaticFiles();

            app.UserOryxWebSocket(options =>
            {
                options.Register("/ws", serviceProvider.GetService<TestHandler>());
                options.Register("/wxqcheck", serviceProvider.GetService<WxqcheckHandler>());
                options.Register("/wxq", serviceProvider.GetService<WxqHandler>());
            });
            //var concurrencyQueue = new ConcurrentQueue<HttpContext>();
            //var concurrencyQueue = new ConcurrentQueue<Func<Task>>();
            //var concurrentProcess = 0;
            //var concurrentProcessLimit = 12;
            //app.Use(async (ctx, next) =>
            //{
            //    concurrencyQueue.Enqueue(next);

            //    while (!concurrencyQueue.IsEmpty)
            //    {
            //        if (concurrentProcess < concurrentProcessLimit)
            //        {
            //            Interlocked.Increment(ref concurrentProcess);
            //        }
            //        else
            //        {
            //            continue;
            //        }
            //        Func<Task> _next;
            //        if (concurrencyQueue.TryDequeue(out _next))
            //        {
            //            await _next();
            //            Interlocked.Decrement(ref concurrentProcess);
            //        }
            //    }
            //});
            //app.UseMaxConcurrentRequests();
            app.Use(async (ctx, next) =>
            {
                await ctx.Session.LoadAsync();
                ctx.Session.SetString("testUser", "true");
                await ctx.Session.CommitAsync();
                if (ctx.Request.Path.ToString().ToLower() == "/account/login" ||
              ctx.Request.Path.ToString().ToLower().Contains("hugescreen") ||
              ctx.Request.Path.ToString().ToLower().Contains("client") ||
              ctx.Request.Query["key"] == "Linengneng" ||
              ctx.Request.Headers["Referer"].Any(x => x.Contains("servicewechat.com")) ||
              ctx.Request.Path.ToString().ToLower().Contains("api") ||
              ctx.Request.Path.ToString().ToLower().Contains("wxq") ||
              ctx.Request.Path.ToString().ToLower().Contains("wxqpic") ||
              ctx.Request.Path.ToString().ToLower().Contains("HugeScreen") ||
              ctx.Session.GetString("loginUser") == "true")
                {
                    await next();
                }
                else
                {
                    ctx.Response.Redirect("/account/login");
                }
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                                         name: "default",
                                         template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
