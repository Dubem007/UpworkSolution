using Microsoft.AspNetCore.Http;

namespace UserServices.Services.AWS
{
    public interface IAwsS3Client
    {
        Task<bool> RemoveObject(string fileName);
        Task<string> UploadFileAsync(IFormFile formFile);
    }
}
