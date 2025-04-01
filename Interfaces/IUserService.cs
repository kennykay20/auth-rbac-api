using crud_api.Dtos;
using crud_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Interfaces;

public interface IUserService
{
  public Task<ApiResponse<User>> CreateUserAsync(CreateUserDto createUserDto);
  public Task<ApiResponse<User?>> GetUserByEmailAsync(string email);
  public Task<ApiListResponse<User>> GetUserListAsync();
  public Task<ApiListPageResponse<User>> GetUserPageListAsync(int pageNum, int pageSize);
}