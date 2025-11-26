using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Xunit.Sdk;

namespace TravelBooking.Tests.Integration.Helpers;

public static class TokenGenerator
{
    private static string GenerateToken(string role, Guid? id)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, role),
            new(ClaimTypes.NameIdentifier, id.ToString() ?? Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsAVeryLongTestKey_ForIntegrationTests_1234567890!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }

    public static HttpClient AddAuthHeader(this HttpClient client, string role, Guid? id)
    {
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", GenerateToken(role, id));
        return client;
    }
}

public static class AdminTokenGenerator
{
    private static string GenerateAdminToken()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "admin"),
            new Claim(ClaimTypes.Role, "Admin"),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsAVeryLongTestKey_ForIntegrationTests_1234567890!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }

    public static HttpClient AddAdminAuthHeader(this HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", GenerateAdminToken());
        return client;
    }

}