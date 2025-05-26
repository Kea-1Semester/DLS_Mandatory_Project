namespace FileTransferService.Model
{
    public class FileMetaDataModel
    {
        public Guid Id { get; set; }
        public string FileKey { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long Size { get; set; }
        public string UploadedBy { get; set; }
        public DateTimeOffset UploadedAt { get; set; }
        public string MetadataJson { get; set; }
    }
}
