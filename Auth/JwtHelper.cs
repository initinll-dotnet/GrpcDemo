using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Auth;

public class JwtHelper
{
    private const string SECRET_KEY = "b6a88e60-11c8-44b4-a7f7-9b39c070eb65";

    public static SymmetricSecurityKey SecurityKey =>
        new SymmetricSecurityKey(GetSecurityKeyBytes(SECRET_KEY));

    private static byte[] GetSecurityKeyBytes(string secretKey)
    {
        if (secretKey.Length < 16)
        {
            throw new ArgumentOutOfRangeException("Key length is too short. Minimum required is 128 bits (16 bytes).");
        }

        var key = Encoding.ASCII.GetBytes(SECRET_KEY);
        return key;
    }

    public static string GenerateJwtToken(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException("Name is not specified");
        }

        var tokenHandler = new JwtSecurityTokenHandler();

        var key = GetSecurityKeyBytes(SECRET_KEY);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, "Admin")
            }),
            Expires = DateTime.Now.AddMinutes(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static bool ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = GetSecurityKeyBytes(SECRET_KEY);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
