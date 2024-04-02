
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace TeamSpecs.RideAlong.Middleware
{
    public class CorsPreflight
    {
        private readonly RequestDelegate _next;
        public CorsPreflight(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method.Equals("OPTIONS", StringComparison.OrdinalIgnoreCase))
            {
                context.Response.StatusCode = 204;
                context.Response.Headers["Access-Control-Allow-Origin"] = "*"; // Replace "url" with your desired URL
                context.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,OPTIONS"; // Add more methods as needed
                context.Response.Headers["Access-Control-Allow-Headers"] = "*"; // Specify the allowed headers
                context.Response.Headers["Access-Control-Allow-Credentials"] = "true";
                return;
            }

            await _next(context);
        }
    }
    public static class CorsPreflightExtensions
    {
        public static IApplicationBuilder useCorsPreflight(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsPreflight>();
        }
    }
}
