using crud_api.Dtos;
using crud_api.Models;

namespace crud_api.Interfaces;

public interface IAuthService
{
  public Task<ApiResponse<User?>> RegisterUserAsync(CreateUserDto createUserDto);
  public Task<ApiTokenResponse> LoginUserAsync(LoginDto loginDto);
  public Task<ApiResponse<User>> GenerateNewOtpAsync(string uuid);
  public Task<ApiResponse<User>> VerifyOtpAsync(string uuid, string userOtp);
  public Task<ApiTokenResponse> GenerateNewAccessToken(string refreshToken);
  public Task<User> ValidateRefreshToken(Guid uuid, string? refreshToken);
  public Task<ApiTokenResponse?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest);
}