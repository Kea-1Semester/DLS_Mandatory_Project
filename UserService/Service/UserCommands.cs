using UserService.DbContext;
using UserService.Models.CQRS;
using UserService.Models.DTO;

namespace UserService.Service
{
    public class UserCommands
    {
        private readonly ObjectDbContext _repo;
        public UserCommands(ObjectDbContext repo)
        {
            _repo = repo;

        }

        public async Task SaveUser(UserInfo userModel)
        {
          
               
                var user = await _repo.GetOrInsertUser(userModel.Guid);

                // we need to get the last user description to update it
                var lastUserDescribtion = user.UserDescriptions
                    .OrderByDescending(description => description.ModifiedDate)
                    .FirstOrDefault();

            try 
            { 
                if (lastUserDescribtion is null ||
                   lastUserDescribtion.FirstName != userModel.FirstName ||
                   lastUserDescribtion.LastName != userModel.LastName ||
                   lastUserDescribtion.Email != userModel.Email ||
                   lastUserDescribtion.PhoneNumber != userModel.PhoneNumber ||
                   lastUserDescribtion.UserName != userModel.UserName)
                {
                    // we need to check if the user has been modified by another user in the meantime
                    var modifiedTicks = lastUserDescribtion?.ModifiedDate.Ticks / 10000 * 10000 ?? 0;
                    if (modifiedTicks != userModel.LastModifiedTicks)
                    {
                        throw new Exception("A new update has occurred since you loaded the page. Please refresh and try again.");
                    }

                    //validate the user model

                    userModel.Validate();

                    // Using BCrypt to hash the password
                    // We are using a work factor of 13 to make the hashing process slower and more secure, check if 13 is the best value for our case.
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userModel.Password, workFactor: 13);


                    await _repo.AddAsync(new UserDescription
                    {
                        UserId = user.Id,
                        ModifiedDate = DateTime.UtcNow,
                        FirstName = userModel.FirstName,
                        LastName = userModel.LastName,
                        Email = userModel.Email,
                        PhoneNumber = userModel.PhoneNumber,
                        UserName = userModel.UserName,
                        Password = hashedPassword //userModel.Password
                    });
                    await _repo.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"user model is not valid: {ex.Message}");
            }
           
        }

        public async Task DeleteUser(Guid userGuid)
        {
            var user = await _repo.GetOrInsertUser(userGuid);
            await _repo.AddAsync(new UserRemoved
            {
                RemovedDate = DateTime.UtcNow,
                UserId = user.Id
            });
            await _repo.SaveChangesAsync();

        }

    }
}


