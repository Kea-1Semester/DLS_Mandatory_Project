using Microsoft.EntityFrameworkCore;

namespace ChatClassLibrary
{    
    public class MessageDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<LobbyMessage> LobbyMessages { get; set; }

        public MessageDbContext(DbContextOptions<MessageDbContext> options) : base(options)
        {
        }
    
        protected MessageDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
