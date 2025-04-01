using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("api/v1/authorization")]
[ApiController]
public class AuthorizationController : ControllerBase
{
  [Authorize(Roles = "Admin")]
  [HttpPost("roles/add")]
  public IResult HandleAddRole()
  {
    return Results.Json("Yes");
  }
}