using Amazon.S3;
using Amazon.S3.Transfer;
using crud_api.database;
using crud_api.Interfaces;
using crud_api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace crud_api.Services;

public class S3Service(IConfiguration configuration, DbDataContext context) : IS3Service
{

  // private readonly string _bucketName;

  readonly string BucketName = configuration["AWS:BucketName"]!;
  readonly IAmazonS3 S3Amazon = new AmazonS3Client(
    configuration["AWS:AccessKey"],
    configuration["AWS:SecretKey"],
    Amazon.RegionEndpoint.GetBySystemName(configuration["AWS:Region"])
  );

  public async Task<string> UploadFileAsync(IFormFile file)
  {
    if (file == null || file.Length == 0)
    {
      return "Invalid file.";
    }
    var fileName = $"{Guid.NewGuid}_{file.FileName}";

    using var stream = file.OpenReadStream();
    var uploadRequest = new TransferUtilityUploadRequest
    {
      InputStream = stream,
      Key = fileName,
      BucketName = BucketName,
      ContentType = file.ContentType
    };

    var fileTransferUtility = new TransferUtility(S3Amazon);
    await fileTransferUtility.UploadAsync(uploadRequest);

    return $"https://{BucketName}.s3.amazonaws.com/{fileName}";
  }

  public async Task<string> SaveImages(IFormFile file)
  {
      // Upload to S3
    var s3Url = await UploadFileAsync(file);
    var image = new ImageFile
    {
      FileName = file.FileName,
      S3Url = s3Url
    };

    await context.Images.AddAsync(image);
    await context.SaveChangesAsync();
    return "Image saved successfully!";
  }

  public async Task<List<ImageFile>> GetAllImages() {
    var result = await context.Images.ToListAsync();
    return result;
  }
}