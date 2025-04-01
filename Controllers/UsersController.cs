using Microsoft.AspNetCore.Mvc;
using crud_api.Models;
using Microsoft.AspNetCore.Authorization;
using crud_api.Interfaces;

namespace crud_api.Controllers;

[Route("api/v1/[controller]")]
[ApiController]
public class Users(IUserService userService) : ControllerBase
{

  [Authorize(Roles = "Admin")]
  [HttpGet("list")]
  public async Task<IActionResult> HandleUserList()
  {
    var result = await userService.GetUserListAsync();
    return Ok(result);
  }

  [Authorize]
  [HttpGet("list/page")]
  public async Task<IActionResult> HandleUserPageList([FromQuery]int pageNumber, int pageSize)
  {
    var result = await userService.GetUserPageListAsync(pageNumber, pageSize);
    return Ok(result);
  }

}
