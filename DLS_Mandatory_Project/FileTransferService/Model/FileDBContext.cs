using Microsoft.EntityFrameworkCore;

namespace FileTransferService.Model
{
    public class FileDBContext : DbContext
    {
        public FileDBContext(DbContextOptions options) 
            : base(options) { }
        
        public DbSet<FileMetaDataModel> Files { get; set; }

        
    }
}
