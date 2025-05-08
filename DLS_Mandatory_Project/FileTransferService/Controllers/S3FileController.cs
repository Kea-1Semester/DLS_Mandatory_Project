using FileTransferService.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileTransferService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class S3FileController : ControllerBase
    {
        private readonly IS3Service _s3service;

        public S3FileController(IS3Service s3service)
        {
            _s3service = s3service;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var fileKey = await _s3service.UploadFileAsync(file);
            return Ok(new { Key = fileKey });
        }

        [HttpGet("{key}")]
        public async Task<IActionResult> Download(string key)
        {
            var stream = await _s3service.GetFileAsync(key);
            return File(stream, "application/octet-stream", key);
        }
    }
}
