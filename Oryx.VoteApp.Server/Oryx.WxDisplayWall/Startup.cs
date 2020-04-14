using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Oryx.WebSocket.Extension.Builder;
using Oryx.WebSocket.Extension.DependencyInjection;
using Oryx.WxDisplayWall.Filters;
using Oryx.WxDisplayWall.WsHandler;

namespace Oryx.WxDisplayWall
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(opts =>
            {
                opts.Filters.Add<GlobalExceptionFilter>();
            });
            services.AddOryxWebSocket();
            services.AddSingleton<ProcessWebsocketHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
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

            app.UseStaticFiles();

            app.UserOryxWebSocket(options =>
            {
                options.Register("/ws", serviceProvider.GetService<ProcessWebsocketHandler>());
            });

            app.UseMvc(_config =>
            {
                _config.MapRoute(
                                 name: "default",
                                 template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
