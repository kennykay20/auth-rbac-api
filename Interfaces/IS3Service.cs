using crud_api.Models;

namespace crud_api.Interfaces;

public interface IS3Service
{
  public Task<string> UploadFileAsync(IFormFile file);
  public Task<string> SaveImages(IFormFile file);
  public Task<List<ImageFile>> GetAllImages();
}