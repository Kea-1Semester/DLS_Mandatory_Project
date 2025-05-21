using Microsoft.EntityFrameworkCore;
using System;
using UserClassLibrary;

namespace UserService.DbContext
{
    // Renamed the namespace to avoid conflict with Microsoft.EntityFrameworkCore.DbContext
    public class ObjectDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<User> Users { get; set; }


        public ObjectDbContext(DbContextOptions<ObjectDbContext> options) : base(options)
        {
        }
    

        protected ObjectDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasAlternateKey(v => new { v.Guid });
        }

        /// <summary>
        /// This method is used to get or insert a user into the database.
        /// Her we are using the Guid to identify the user. We are uing idempotance to avoid duplicate entries.
        /// </summary>
        /// <param name="userGuid">The Guid of the user to be inserted or retrieved.</param>
        /// <returns>returns the user object.</returns>
        public async Task<User> GetOrInsertUser(Guid userGuid)
        {
            var user = Users
                .Include(user => user.UserDescriptions)
                .SingleOrDefault(user => user.Guid == userGuid);

            if (user is not null) return user;
            user = new User
            {
                Guid = userGuid
            };
            await AddAsync(user);
            await SaveChangesAsync();
            return user;

        }
    }

}
