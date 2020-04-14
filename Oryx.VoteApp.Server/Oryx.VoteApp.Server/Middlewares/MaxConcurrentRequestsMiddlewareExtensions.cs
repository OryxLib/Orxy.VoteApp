using System;
using Microsoft.AspNetCore.Builder;

namespace Oryx.MaxConcurrentRequests.Middlewares
{
    public static class MaxConcurrentRequestsMiddlewareExtensions
    {
        public static IApplicationBuilder UseMaxConcurrentRequests(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<MaxConcurrentRequestsMiddleware>();
        }
    }
}
