using System.Text;
using System.Security.Cryptography;


namespace crud_api.Utils
{
  public static class PasswordHelper
  {
    public static byte[] GenerateSalt() {
      using var hmac = new HMACSHA256();
      byte[] salt = hmac.Key;
      return salt;
    }
    public static string GenerateHashPassword(string password, byte[] salt) {
      using var hmac = new HMACSHA256(salt);
      byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
      string hashPassword = Convert.ToBase64String(salt) + "." + Convert.ToBase64String(hash);
      return hashPassword;
    }

    public static bool VerifyHashPassword(string password, string hashPassword) {
      try
      {
        var splitHassPassword = hashPassword.Split(".");
        var salt = Convert.FromBase64String(splitHassPassword[0]);
        var storedPasswordHash = Convert.FromBase64String(splitHassPassword[1]);

        using var hmac = new HMACSHA256(salt);
        var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        Console.WriteLine("computeHash line 33: " + Convert.ToBase64String(computeHash));
        return CryptographicOperations.FixedTimeEquals(computeHash, storedPasswordHash);
      }
      catch (Exception ex)
      {
        throw new(ex.Message);
      }
    }
  }
}