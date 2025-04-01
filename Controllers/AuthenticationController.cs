using crud_api.Dtos;
using crud_api.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("api/v1/authentication")]
[ApiController]
public class Auth(IAuthService authService) : ControllerBase
{
  [HttpPost("register")]
  public async Task<IResult> HandleRegister([FromBody] CreateUserDto request)
  {
    if (!ModelState.IsValid)
    {
      return Results.BadRequest(ModelState);
    }
    var result = await authService.RegisterUserAsync(request);
    return Results.Json(result);
  }

  [HttpPost("login")]
  public async Task<IResult> HandleLogin([FromBody] LoginDto request)
  {
    if (!ModelState.IsValid)
    {
      return Results.BadRequest(ModelState);
    }
    var result = await authService.LoginUserAsync(request);
    return Results.Json(result);
  }

  [HttpPost("refresh-token")]
  public async Task<IResult> HandleNewRefreshToken(RefreshTokenRequestDto request)
  {
    var result = await authService.RefreshTokenAsync(request);
    if (result is null || result.AccessToken is null || result.RefreshToken is null)
      return Results.BadRequest("Invalid refresh token");
    return Results.Json(result);
  }

  [HttpGet("otp")]
  public async Task<IResult> HandleNewOtp([FromQuery]string userId)
  {
    var result = await authService.GenerateNewOtpAsync(userId);
    return Results.Json(result);
  }

  [HttpPut("otp/verify")]
  public async Task<IResult> HandleVerifyOtp([FromBody]string userId, string otp)
  {
    var result = await authService.VerifyOtpAsync(userId, otp);
    return Results.Json(result);
  }

  [HttpPost("access-token")]
  public async Task<IResult> HandleNewAccessToken([FromBody]string refreshToken)
  {
    Console.WriteLine("inside the accesstoken handle refreshToken = " + refreshToken);
    var result = await authService.GenerateNewAccessToken(refreshToken);
    return Results.Json(result);
  }

}