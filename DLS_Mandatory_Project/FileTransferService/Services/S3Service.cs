
using Amazon.S3;

namespace FileTransferService.Services
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IConfiguration config)
        {
            var aws = config.GetSection("AWS");
            _bucketName = aws["BucketName"];

            _s3Client = new AmazonS3Client(
                aws["AccessKey"],
                aws["SecretKey"],
                Amazon.RegionEndpoint.GetBySystemName(aws["Region"]));
        }

        Task<string> IS3Service.UploadFileAsync(IFormFile file)
        {
            throw new NotImplementedException();
        }

        Task<Stream> IS3Service.GetFileAsync(string key)
        {
            throw new NotImplementedException();
        }
    }
}
