using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TravelBooking.Application.Interfaces;
using TravelBooking.Application.Interfaces.Security;

namespace TravelBooking.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _config;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly string _secret;
    private readonly int _expiryMinutes;

    public JwtService(IConfiguration config)
    {
        _config = config;
        _issuer = config["Jwt:Issuer"] ?? "TravelBooking";
        _audience = config["Jwt:Audience"] ?? "TravelBookingAudience";
        _secret = config["Jwt:Secret"] ?? throw new ArgumentNullException("Jwt:Secret");
        _expiryMinutes = int.TryParse(config["Jwt:ExpiryMinutes"], out var m) ? m : 60;
    }

    public string CreateToken(string userId, string username, IDictionary<string, string>? extraClaims = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            
        };

        if (extraClaims != null)
        {
            foreach (var kv in extraClaims)
                claims.Add(new Claim(kv.Key, kv.Value));
        }

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}