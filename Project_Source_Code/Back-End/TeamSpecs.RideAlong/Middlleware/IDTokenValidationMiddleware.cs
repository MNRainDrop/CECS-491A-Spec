﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;


namespace TeamSpecs.RideAlong.Middleware
{
    public class IDTokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private string _rideAlongSecretKey = "Ride-Along-Super-secret-string";
        private string _rideAlongIssuer = "Ride Along by Team Specs";
        public IDTokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            string token = context.Request.Headers["Authorization"].First()?.Split(" ").Last() ?? "";
            if (!token.IsNullOrEmpty())
            {
                await AttachUserToContext(context, token);
            }
            await _next(context);
        }
        private async Task AttachUserToContext(HttpContext context, string token)
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
                    ValidIssuer = _rideAlongIssuer
                }, out SecurityToken validatedToken);
            }
            catch (Exception)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Invalid Token");
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

