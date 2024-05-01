using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LotusWebApp.Data.Models;
using Microsoft.IdentityModel.Tokens;

namespace LotusWebApp.Services;

public class TokenService(ILogger<TokenService> logger)
{
    // Specify how long until the token expires
    private const int ExpirationMinutes = 30;

    public string CreateToken(ApplicationUser user)
    {
        var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
        var token = CreateJwtToken(
            CreateClaims(user),
            CreateSigningCredentials(),
            expiration
        );
        var tokenHandler = new JwtSecurityTokenHandler();

        logger.LogInformation("JWT Token created");

        return tokenHandler.WriteToken(token);
    }

    public string? ValidateToken(string? token)
    {
        if (token == null)
            return null;

        var secret = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["SymmetricSecurityKey"];
        if (string.IsNullOrEmpty(secret))
        {
            throw new Exception("not configured JwtRegisteredClaimNamesSub in JwtTokenSettings");
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.FromMinutes(30)
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "id").Value;

            // return user id from JWT token if validation successful
            return userId;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }

    private static JwtSecurityToken CreateJwtToken(IEnumerable<Claim> claims, SigningCredentials credentials,
        DateTime expiration) =>
        new(
            new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["ValidIssuer"],
            new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["ValidAudience"],
            claims,
            expires: expiration,
            signingCredentials: credentials
        );

    private static List<Claim> CreateClaims(ApplicationUser user)
    {
        var jwtSub = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["JwtRegisteredClaimNamesSub"];
        if (string.IsNullOrEmpty(jwtSub))
        {
            throw new Exception("not configured JwtRegisteredClaimNamesSub in JwtTokenSettings");
        }

        try
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, jwtSub),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                new(ClaimTypes.NameIdentifier, user.Id),
                new("id", user.Id),
                new(ClaimTypes.Name, user.UserName ?? "no username"),
                new(ClaimTypes.Email, user.Email  ?? "no email"),
                new(ClaimTypes.Role, user.Role.ToString())
            };

            return claims;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private SigningCredentials CreateSigningCredentials()
    {
        var symmetricSecurityKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["SymmetricSecurityKey"];
        if (string.IsNullOrEmpty(symmetricSecurityKey))
        {
            throw new Exception("not configured SymmetricSecurityKey in JwtTokenSettings");
        }
        return new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(symmetricSecurityKey)
            ),
            SecurityAlgorithms.HmacSha256
        );
    }
}