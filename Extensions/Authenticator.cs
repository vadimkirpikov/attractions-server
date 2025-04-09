using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace TouristServer.Extensions;

public static class Authenticator
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfigurationSection jwtSettings)
    {
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(ab =>
            ab.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
            });
        return services;
    }
}