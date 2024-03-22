using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
namespace TeamSpecs.RideAlong.Middleware
{
    public class CorsMiddleware
    {
        private readonly RequestDelegate _next;
        public CorsMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            context.Response.Headers["Access-Control-Allow-Origin"] = "*"; // Replace "url" with your desired URL
            context.Response.Headers["Access-Control-Allow-Methods"] = "GET,POST,OPTIONS"; // Add more methods as needed
            context.Response.Headers["Access-Control-Allow-Headers"] = "*"; // Specify the allowed headers
            context.Response.Headers["Access-Control-Allow-Credentials"] = "true";

            await _next(context);
        }
    }
    public static class CorsMiddlewareExtensions
    {
        public static IApplicationBuilder useCorsMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorsMiddleware>();
        }
    }
}
