using Microsoft.EntityFrameworkCore;
using UserClassLibrary;
using UserService.DbContext;

namespace UserService.Service
{
    public class UserQueries
    {
        private readonly ObjectDbContext _repo;

        public UserQueries(ObjectDbContext repo)
        {
            _repo = repo;
        }
        /// <summary>
        /// This method is used to get alle useres from the database and we are using OrderByDescending to get the last modified user.
        /// </summary>
        /// <returns>Returns a list of user objects.</returns>
        public async Task<List<UserInfo>> GetUsers()
        {
            var result = await _repo.Users
                .Where(user => !user.UserRemoved.Any())
                .Select(user => new
                {
                    user.Guid,
                    UserDescription = user.UserDescriptions
                        .OrderByDescending(d => d.ModifiedDate)
                        .FirstOrDefault()
                }).ToListAsync();
            return result
                .Select(row => MapUser(row.Guid, row.UserDescription))
                .ToList();
        }


        /// <summary>
        /// This meth
        /// </summary>
        /// <param name="userGuid"></param>
        /// <returns></returns>
        public async Task<UserInfo> GetUser(Guid userGuid)
        {
            var result = await _repo.Users
                .Where(user => user.Guid == userGuid &&
                               !user.UserRemoved.Any())
                .Select(user => new
                {
                    user.Guid,
                    UserDescription = user.UserDescriptions
                        .OrderByDescending(d => d.ModifiedDate)
                        .FirstOrDefault()
                })
                .SingleOrDefaultAsync();
            return result == null ? null : MapUser(result.Guid, result.UserDescription);
        }

        public async Task<object> GetUserByEmail(string email)
        {
            var result = await _repo.Users
                .Where(user => user.UserDescriptions
                    .Any(description => description.Email == email) &&
                               !user.UserRemoved.Any())
                .Select(user => new
                {
                    user.Guid,
                    UserDescription = user.UserDescriptions
                        .OrderByDescending(d => d.ModifiedDate)
                        .FirstOrDefault()
                })
                .SingleOrDefaultAsync();

            if (result == null)
            {
                return null;
            }

            return new
            {
                Guid = result.Guid,
                Email = result.UserDescription!.Email,
                HashPassword = result.UserDescription.Password,
                UserRoleCsv = result.UserDescription.RoleCsv,
            };
        }

        private UserInfo MapUser(Guid userGuid, UserDescription description)
        {
            return new UserInfo
            {
                Guid = userGuid,
                FirstName = description.FirstName,
                LastName = description.LastName,
                Email = description.Email,
                PhoneNumber = description.PhoneNumber,
                LastModifiedTicks = description?.ModifiedDate.Ticks / 10000 * 10000 ?? 0,
                UserName = description.UserName,

            };
        }

    }
}
