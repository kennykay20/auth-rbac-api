using crud_api.Interfaces;
using crud_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace crud_api.Controllers;

[Route("/api/v1/[controller]")]
[ApiController]
public class ImagesController(IS3Service _s3Service) : ControllerBase
{
  [HttpPost("photos/upload")]
  public async Task<IActionResult> HandlePhotoUpload([FromForm] IFormFile file)
  {
    var result = await _s3Service.SaveImages(file);
    return Ok(result);
  }
}