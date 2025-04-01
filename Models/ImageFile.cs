namespace crud_api.Models;

public class ImageFile
{
  public int Id { get; set; }
  public string? FileName { get; set; } = string.Empty;
  public string? S3Url { get; set; } = string.Empty;
  public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}