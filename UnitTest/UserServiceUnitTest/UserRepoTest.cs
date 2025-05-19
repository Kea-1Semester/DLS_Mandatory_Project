using Microsoft.EntityFrameworkCore;
using UserService.DbContext;
using UserService.Models.DTO;
using UserService.Service;

namespace UnitTest.UserServiceUnitTest
{
    [TestClass]
    public class UserRepoTest
    {
        private ObjectDbContext _repo;
        private UserCommands _userCommands;
        private UserQueries _userQueries;
        private UserInfo _user;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ObjectDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Ensures isolation per test
                .Options;

            _repo = new ObjectDbContext(options);
            _userCommands = new UserCommands(_repo);
            _userQueries = new UserQueries(_repo);

            _user = new UserInfo
            {
                Guid = Guid.NewGuid(),
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "12345678",
                Email = "john.doe@example.com",
                Password = "Sh12345678#",
                UserName = "Joni"
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _repo.Dispose();
        }

        [TestMethod]
        public async Task WhenAddUserTwice_OneUserIsAdded()
        {
            // Act
            await _userCommands.SaveUser(_user);
            await _userCommands.SaveUser(_user); // Should not add a second one

            var userFromDb = await _userQueries.GetUser(_user.Guid);
            var allUsers = await _userQueries.GetUsers();

            // Assert
            Assert.IsNotNull(userFromDb);
            Assert.AreEqual(_user.Guid, userFromDb.Guid);
            Assert.AreEqual(1, allUsers.Count); // Ensure only one user was added
        }

        [TestMethod]
        public async Task WhenUserModifyEmail_UserNewEmailIsReturned()
        {
            // Arrange
            var user1 = "john.doe@example.com";
            _user.Email = user1;

            // Act  
            await _userCommands.SaveUser(_user);
            var userFromDb = await _userQueries.GetUser(_user.Guid);

            // Arrange
            _user.Email = "john.doeV2@example.com";
            _user.LastModifiedTicks = userFromDb.LastModifiedTicks;
            
            // Act
            await _userCommands.SaveUser(_user);
            var userFromDb2 = await _userQueries.GetUser(_user.Guid);

            // Assert
            Assert.AreNotEqual(user1, userFromDb2.Email); // new email should be different

            Assert.AreEqual(_user.Email, userFromDb2.Email); // new email should be the same

        }

        [TestMethod]
        public async Task WhenSetUserDescription_UserDescriptionIsReturned()
        {

            // Act
            await _userCommands.SaveUser(_user);
            var userFromDb = await _userQueries.GetUser(_user.Guid);

            // Assert
            Assert.IsNotNull(userFromDb);
            Assert.AreEqual(_user.FirstName, userFromDb.FirstName);
        }

        [TestMethod]
        public async Task WhenSetUserToSameDescription_NothingIsSaved()
        {
            // Arrange
            await _userCommands.SaveUser(_user);
            var firstSnapshot = await _userQueries.GetUser(_user.Guid);

            // Act
            _user.LastModifiedTicks = firstSnapshot.LastModifiedTicks;
            await _userCommands.SaveUser(_user); // Saving with same LastModifiedTicks
            var secondSnapshot = await _userQueries.GetUser(_user.Guid);

            // Assert
            Assert.AreEqual(firstSnapshot.LastModifiedTicks, secondSnapshot.LastModifiedTicks);
        }
        [TestMethod]
        public async Task WhenUserIsModifiedConcurrently_ExceptionIsThrown()
        {
            // Arrange
            await _userCommands.SaveUser(_user);
            var user = await _userQueries.GetUser(_user.Guid);

            // Change 1
            _user.FirstName = "Jack";
            _user.LastModifiedTicks = user.LastModifiedTicks;
            await _userCommands.SaveUser(_user);

            // Change 2
            _user.FirstName = "Max";
            _user.LastModifiedTicks = user.LastModifiedTicks;
            Func<Task> concurrentSave = () => _userCommands.SaveUser(_user);

            // Throw exception
           await Assert.ThrowsExceptionAsync<Exception>(() => concurrentSave());
        }
        [TestMethod]
        public async Task UserDeleteVenue_UserIsNotReturned()
        {
            // Act
            _userCommands.SaveUser(_user).Wait();
            var currentUser = await _userQueries.GetUser(_user.Guid);
            _userCommands.DeleteUser(currentUser.Guid).Wait();

            // Assert
            var getCurrentUserById = await _userQueries.GetUser(currentUser.Guid);
            Assert.IsNull(getCurrentUserById);

            // Assert
            var alleUser = await _userQueries.GetUsers();
            Assert.AreEqual(0, alleUser.Count);

        }
    }
}
