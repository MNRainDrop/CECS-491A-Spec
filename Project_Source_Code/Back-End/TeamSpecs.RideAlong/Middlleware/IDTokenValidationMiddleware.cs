using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;


namespace TeamSpecs.RideAlong.Middleware
{
    public class IDTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private string _rideAlongSecretKey = "This is Ridealong's super secret key for testing security";
        private string _rideAlongIssuer = "Ride Along by Team Specs";
        public IDTokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                string token = (string)context.Request.Headers["Authorization"].First()?.Split(" ").Last()!;
                if (!token.IsNullOrEmpty())
                {
                    await ValidateTheToken(context, token);
                }
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("No Token Provided");
            }
        }
        private async Task ValidateTheToken(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_rideAlongSecretKey);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _rideAlongIssuer,
                    ValidateAudience = false,
                }, out SecurityToken validatedToken);
                await _next(context);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid Token: " + ex.Message);
            }
        }
    }
    public static class IDTokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder useIDValidator(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IDTokenValidationMiddleware>();
        }
    }
}

