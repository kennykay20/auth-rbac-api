using crud_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[ApiController]
[Route("api/v1/tests")]
public class TestDemoController(TestDemo logic) : ControllerBase
{
  [HttpGet("value")]
  public IResult GetValues()
  {
    var result = "value1: " + logic.Value1 + " - " + " value2: " + logic.Value2;
    return Results.Ok(result);
  }
}