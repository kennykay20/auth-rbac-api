namespace crud_api.Utils
{
  public static class Otps
  {
    public static string GenerateOtp() {
      return Random.Shared.Next(111111, 999999).ToString();
    }
  }
}