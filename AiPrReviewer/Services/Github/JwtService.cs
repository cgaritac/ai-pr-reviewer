using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace AiPrReviewer.Services.Github;

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
        
        // Crear el RSA y mantenerlo vivo mientras el servicio exista
        _rsa = RSA.Create();
        _rsa.ImportFromPem(privateKey.ToCharArray());
        _rsaSecurityKey = new RsaSecurityKey(_rsa);
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

            var token = new JwtSecurityToken(
                issuer: _appId,
                expires: DateTime.UtcNow.AddMinutes(10).AddSeconds(-30),
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