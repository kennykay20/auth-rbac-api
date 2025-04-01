using crud_api.database;
using crud_api.Dtos;
using crud_api.Interfaces;
using crud_api.Models;
using crud_api.Utils;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class AuthService(IUserService userService, DbDataContext context, IConfiguration configuration) : IAuthService
{
    public async Task<ApiResponse<User?>> RegisterUserAsync(CreateUserDto createUserDto)
    {
        ApiResponse<User?> existEmail;
        var email = createUserDto.Email;
        var password = createUserDto.Password;
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) {
          return new ApiResponse<User?>(){
            Status = 400,
            Success = false,
            Message = "Please enter either/both email or password",
            Data = null
          };
        }

        // check if email already exist
        existEmail = await userService.GetUserByEmailAsync(email);

        if (existEmail.Data != null) {
          return new ApiResponse<User?>()
          {
            Status = 400,
            Success = false,
            Message = "Email already exist",
            Data = null
          };
        }
        var user = await userService.CreateUserAsync(createUserDto);
        try
        {
          // generate an OTP
          string otp = Otps.GenerateOtp();
          // send OTP to user's email or as a text message to phone number
          DateTime otpExpiry = DateTime.UtcNow.AddMinutes(10);
          user.Data!.Otp = otp;
          user.Data.OtpExpiry = otpExpiry;

          // update user table
          context.Entry(user.Data).CurrentValues.SetValues(user.Data);
          await context.SaveChangesAsync();

          return new ApiResponse<User?>()
          {
            Success = true,
            Status = 201,
            Message = "New User registered successfully, Please check your email to verify your account",
            Data = user.Data
          };
        }
        catch (Exception ex)
        {
          throw new(ex.Message);
        }
    }
    public async Task<ApiTokenResponse> LoginUserAsync(LoginDto loginDto)
    {
      bool isMatch = false;
      var email = loginDto.Email;
      var password = loginDto.Password;
      try
      {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)) {
          return new ApiTokenResponse()
          {
            Status = 400,
            Success = false,
            Message = "Please enter either/both email or password",
            AccessToken = "",
            RefreshToken = "",
          };
        }

        var user = await userService.GetUserByEmailAsync(email);
        if (user.Data is null) {
          return new ApiTokenResponse()
          {
            Status = 404,
            Success = false,
            Message = "User not found. Please sign up or register",
            AccessToken = "",
            RefreshToken = "",
          };
        }
        if (!user.Data!.IsActive) {
          if (user.Data.IsNewUser) {
            return new ApiTokenResponse()
            {
              Status = 404,
              Success = false,
              Message = "Account not verified, please check your email for verification link",
              AccessToken = "",
              RefreshToken = "",
            };
          }
          else {
            return new ApiTokenResponse()
            {
              Status = 404,
              Success = false,
              Message = "User has been de-activated",
              AccessToken = "",
              RefreshToken = "",
            };
          }
        }
        var hashPassword = user.Data.Password;
        if (!string.IsNullOrEmpty(hashPassword)) {
          isMatch = PasswordHelper.VerifyHashPassword(password, hashPassword);
        }

        if(!isMatch) {
          return new ApiTokenResponse()
          {
            Status = 400,
            Success = false,
            Message = "Invalid Password",
            AccessToken = "",
            RefreshToken = "",
          };
        }
        var token = await GenerateAccessToken(user.Data.Uuid.ToString());
        user.Data.RegistrationToken = token ?? "";

        context.Entry(user.Data).CurrentValues.SetValues(user.Data);
        context.SaveChanges();

        var newData = await GenerateNewRefreshToken(user.Data.Uuid.ToString());
        return new ApiTokenResponse()
        {
          Success = true,
          Message = "Login Successfully!",
          Status = 200,
          AccessToken = token ?? "",
          RefreshToken = newData.RefreshToken ?? ""
        };
      }
      catch (Exception ex)
      {
        throw new(ex.Message);
      }
    }

    private async Task<string> GenerateAccessToken(string uuid)
    {
      var user = await context.Users.FirstOrDefaultAsync((user) => user.Uuid.ToString() == uuid);
      if (user is null) {
        return null!;
      }
      var keyVal = configuration.GetValue<string>("Jwt:Key")!;
      var issuerVal = configuration.GetValue<string>("Jwt:Issuer")!;
      var audienceVal = configuration.GetValue<string>("Jwt:Audience")!;

      var token = Jwt.GenerateAccessToken(user!, keyVal, issuerVal, audienceVal);
      return token;
    }
    public async Task<ApiTokenResponse> GenerateNewAccessToken(string refreshToken)
    {
      if (string.IsNullOrEmpty(refreshToken))
      {
        return new ApiTokenResponse()
        {
          Success = false,
          Message = "refresh token is required",
          Status = 400,
          AccessToken = "",
          RefreshToken = "",
        };
      }
      var user = await context.Users.FirstOrDefaultAsync((user) => user.RefreshToken == refreshToken);
      if (user is null) {
        return new ApiTokenResponse()
        {
          Success = false,
          Message = "User not found",
          Status = 404,
          AccessToken = "",
          RefreshToken = "",
        };
      }
      if (user.RefreshTokenExpiryTime <= DateTime.UtcNow) {
        return new ApiTokenResponse()
        {
          Success = false,
          Message = "RefreshToken has expired.",
          Status = 401,
          AccessToken = "",
          RefreshToken = "",
        };
      }
      var keyVal = configuration.GetValue<string>("Jwt:Key")!;
      var issuerVal = configuration.GetValue<string>("Jwt:Issuer")!;
      var audienceVal = configuration.GetValue<string>("Jwt:Audience")!;

      var token = Jwt.GenerateAccessToken(user!, keyVal, issuerVal, audienceVal);

      user.RegistrationToken = token ?? "";
      await context.SaveChangesAsync();

      return new ApiTokenResponse()
      {
        Message = "Access Token Generated",
        Success = true,
        Status = 200,
        AccessToken = token!,
        RefreshToken = "",
      };
    }
    public async Task<ApiResponse<User>> GenerateNewOtpAsync(string uuid)
    {
      var user = await context.Users.FirstOrDefaultAsync((usr) => usr.Uuid.ToString() == uuid);
      if (user is null) {
        return new ApiResponse<User>()
        {
          Success = false,
          Message = "User not found",
          Status = 404,
          Data = null
        };
      }

      user!.Otp = Otps.GenerateOtp();;
      user.OtpExpiry = DateTime.UtcNow.AddMinutes(10);

      context.Entry(user).CurrentValues.SetValues(user);
      context.SaveChanges();

      return new ApiResponse<User>()
      {
        Success = true,
        Message = "New Otp generated and send to user's email!",
        Status = 201,
        Data = user,
      };
    }
    public async Task<ApiResponse<User>> VerifyOtpAsync(string uuid, string userOtp)
    {
      try
      {
        var user = await context.Users.FirstOrDefaultAsync((usr) => usr.Uuid.ToString() == uuid);
        if (user is null) {
          return new ApiResponse<User>()
          {
            Success = false,
            Status = 404,
            Message = "User not found",
            Data = null
          };
        }
        if (user.IsActive) {
          // user is verified
          return new ApiResponse<User>()
          {
            Success = false,
            Message = "User is already verified!",
            Status = 201,
            Data = null
          };
        }
        if (user.Otp != userOtp || user.OtpExpiry <= DateTime.UtcNow.AddMinutes(10)) {
          return new ApiResponse<User>()
          {
            Success = false,
            Message = "Invalid or expired OTP",
            Status = 401,
            Data = null
          };
        }

        user.IsActive = true;
        user.IsNewUser = true;
        user.Otp = "";
        user.OtpExpiry = null;

        context.Entry(user).CurrentValues.SetValues(user);
        context.SaveChanges();

        user.Password = "";
        // user
        return new ApiResponse<User>()
        {
          Success = true,
          Message = "Email verified successfully, you can now login",
          Status = 200,
          Data = user
        };
      }
      catch (Exception ex)
      {
        throw new(ex.Message);
      }
    }
    private async Task<ApiTokenResponse> GenerateNewRefreshToken(string uuid)
    {
      var user = await context.Users.FirstOrDefaultAsync(usr => usr.Uuid.ToString() == uuid);
      if (user is null) {
        return new ApiTokenResponse()
        {
          Success = false,
          Message = "User not found",
          Status = 404,
          AccessToken = "",
          RefreshToken = "",
        };
      }

      var refreshToken = Jwt.GenerateRefreshToken();
      user!.RefreshToken = refreshToken;
      user.RefreshTokenExpiryTime = DateTime.UtcNow.AddHours(1);
      await context.SaveChangesAsync();
      return new ApiTokenResponse()
      {
        Success = false,
        Status = 200,
        Message = "Refresh Token Generated",
        AccessToken = "",
        RefreshToken = refreshToken,
      };
    }

    public async Task<User> ValidateRefreshToken(Guid uuid, string? refreshToken)
    {
      var user = await context.Users.FirstOrDefaultAsync((usr) => usr.Uuid == uuid);
      if((user is null) || user!.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
      {
        return null!;
      }
      return user;
    }

    public async Task<ApiTokenResponse?> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
      var user = await ValidateRefreshToken(request.Uuid, request.RefreshToken);
      if (user is null)
        return null;
      var AccessTokenData = await GenerateNewAccessToken(request.RefreshToken);
      var refreshData = await GenerateNewRefreshToken(request.Uuid.ToString());
      return new ApiTokenResponse()
      {
        AccessToken = AccessTokenData.AccessToken,
        RefreshToken = refreshData.RefreshToken,
      };
    }
}