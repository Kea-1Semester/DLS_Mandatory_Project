using Microsoft.AspNetCore.Http;

namespace FileTransferService.Services
{
    public interface IS3Service
    {
        Task<string> UploadFileAsync(IFormFile file);
        Task<Stream> GetFileAsync(string key);
    }
}
