using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AiPrReviewer.Infrastructure.Github;

public class JwtService : IDisposable
{
    private readonly string _appId;
    private readonly RSA _rsa;
    private readonly RsaSecurityKey _rsaSecurityKey;

    public JwtService()
    {
        _appId = Environment.GetEnvironmentVariable("APP_ID") 
            ?? throw new Exception("APP_ID is not set. Configure it in the .env file or as an environment variable.");
        
        var privateKeyRaw = Environment.GetEnvironmentVariable("PRIVATE_KEY") 
            ?? throw new Exception("PRIVATE_KEY is not set. Configure it in the .env file or as an environment variable.");
        
        var privateKey = privateKeyRaw.Replace("\\n", "\n");
        
        if (!privateKey.Contains("BEGIN"))
        {
            privateKey = $"-----BEGIN RSA PRIVATE KEY-----\n{privateKey}\n-----END RSA PRIVATE KEY-----";
        }
        
        try
        {
            _rsa = RSA.Create();
            _rsa.ImportFromPem(privateKey.ToCharArray());
            _rsaSecurityKey = new RsaSecurityKey(_rsa);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error processing private key. Verify that PRIVATE_KEY is in PEM format. Error: {ex.Message}", ex);
        }
    }
    
    public void Dispose()
    {
        _rsa?.Dispose();
    }

    public string GenerateJwtToken()
    {
        try
        {
            var credentials = new SigningCredentials(
                _rsaSecurityKey,
                SecurityAlgorithms.RsaSha256
            );

            var now = DateTime.UtcNow;
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Iss, _appId),
                new Claim(JwtRegisteredClaimNames.Iat, 
                    new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), 
                    ClaimValueTypes.Integer64)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(10).AddSeconds(-30),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error generating JWT token: {ex.Message}");
            throw new Exception("Error generating JWT token", ex);
        }
    }
}