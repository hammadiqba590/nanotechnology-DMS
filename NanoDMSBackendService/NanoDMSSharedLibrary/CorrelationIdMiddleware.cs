using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoDMSSharedLibrary
{
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            context.Response.Headers["X-Correlation-ID"] = correlationId;
            context.TraceIdentifier = correlationId;

            await _next(context);
        }
    }
}
