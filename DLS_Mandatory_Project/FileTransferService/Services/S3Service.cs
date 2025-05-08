
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;

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

        async Task<string> IS3Service.UploadFileAsync(IFormFile file)
        {
            var fileKey = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),
                Key = fileKey,
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            return fileKey;
        }

        async Task<Stream> IS3Service.GetFileAsync(string key)
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.GetObjectAsync(request);
            return response.ResponseStream;
        }
    }
}
