using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using crud_api.Models;
using Microsoft.IdentityModel.Tokens;

namespace crud_api.Utils;

public static class Jwt
{
  public static string[] GetSecretKeys() {
    string ACCESS_TOKEN_SECRET = "";
    string REFRESH_TOKEN_SECRET = "";
    return [ACCESS_TOKEN_SECRET, REFRESH_TOKEN_SECRET];
  }

  public static string GenerateAccessToken(User user, string keyValue, string issuerValue, string audienceValue) {
    var key = Encoding.UTF8.GetBytes(keyValue);
    var claims = new[]
    {
      new Claim(JwtRegisteredClaimNames.Sub, Convert.ToString(user.Id)),
      new Claim(JwtRegisteredClaimNames.Email, user.Email!),
      new Claim(ClaimTypes.NameIdentifier, user.Uuid.ToString()),
      new Claim(ClaimTypes.Role, user.Roles),
    };

    var token = new JwtSecurityToken(
      issuer: issuerValue,
      audience: audienceValue,
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(30),
      signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public static string GenerateRefreshToken()
  {
    var randomNumber = new byte[32];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(randomNumber);
    return Convert.ToBase64String(randomNumber);
  }
}