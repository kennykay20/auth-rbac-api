
namespace crud_api.Models;
  public class User
  {
    public int Id { get; set; }
    public Guid Uuid { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public string Roles { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsNewUser { get; set; }
    public string Otp { get; set; } = string.Empty;
    public DateTime? OtpExpiry { get; set; }
    public string RegistrationToken { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }
  }
