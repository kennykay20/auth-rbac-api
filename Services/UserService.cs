using crud_api.database;
using crud_api.Dtos;
using crud_api.Interfaces;
using crud_api.Models;
using crud_api.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class UserService(DbDataContext context, IPasswordHasher<User> passwordHasher) : IUserService
{
    public async Task<ApiResponse<User>> CreateUserAsync(CreateUserDto request)
    {
      try
      {
        var salt = PasswordHelper.GenerateSalt();
        var passwordHash = PasswordHelper.GenerateHashPassword(request.Password!.Trim(), salt);

        User user = new User();
        user.Uuid = Guid.NewGuid();
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Password = passwordHash;
        user.IsActive = false;
        user.IsNewUser = true;
        user.IsDeleted = false;
        user.Roles = "Admin";

        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();

        return new ApiResponse<User>()
        {
          Success = true,
          Status = 201,
          Data = user
        };
      }
      catch (Exception ex)
      {
        throw new(ex.Message);
      }
    }

    public async Task<ApiResponse<User?>> GetUserByEmailAsync(string email)
    {
      try
      {
          var user = await context.Users.FirstOrDefaultAsync((user) => user.Email == email!.ToLower() && user.IsDeleted == false);
          if (user is null)
          {
            return new ApiResponse<User?>()
            {
              Success = false,
              Status = 400,
              Data = null
            };
          }
          return new ApiResponse<User?>()
          {
            Success = true,
            Status = 200,
            Data = user
          };
      }
      catch (Exception ex)
      {
        throw new(ex.Message);
      }
    }

    public async Task<ApiListResponse<User>> GetUserListAsync()
    {
      List<User> users = await context.Users.ToListAsync();
      if (users is null)
      {
        return new ApiListResponse<User>()
        {
          Success = false,
          Status = 400,
          Data = null
        };
      }
      foreach (var item in users)
      {
        item.Password = "";
        item.RegistrationToken = "";
      }
      return new ApiListResponse<User>() {
        Success = true,
        Status = 200,
        Data = users
      };
    }

    public async Task<ApiListPageResponse<User>> GetUserPageListAsync(int pageNum, int pageSize)
    {
      if (pageNum < 1 || pageSize < 1)
      {
        return new ApiListPageResponse<User>()
        {
          Status = 400,
          Success = false,
          Message = "Invalid pagination parameters.",
          Data = null,
        };
      }
      var users = context.Users.AsQueryable();
      var total = await users.CountAsync();

      foreach (var item in users)
      {
        item.Password = "";
        item.RegistrationToken = "";
      }

      var items = await users
          .Skip((pageNum - 1) * pageSize)
          .Take(pageSize)
          .ToListAsync();

      return new ApiListPageResponse<User>() {
        Success = true,
        Status = 200,
        PageNumber = pageNum,
        PageSize = pageSize,
        Count = total,
        TotalPages = (int)Math.Ceiling((double)total/pageSize),
        Data = items
      };
    }

    private string PasswordWordHasher(User user, string password)
    {
      return passwordHasher.HashPassword(user, password);
    }

    private bool VerifyHashedPassword(User user, string hashedPassword, string password) {
      return passwordHasher.VerifyHashedPassword(user, hashedPassword, password) == PasswordVerificationResult.Success;
    }
}