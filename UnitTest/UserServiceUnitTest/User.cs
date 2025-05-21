using System.Xml.Linq;
using UserClassLibrary;

namespace UnitTest.UserServiceUnitTest
{
    [TestClass]
    public sealed class User
    {

        private UserModel user;


        [TestInitialize]
        public void Initialize()
        {
            user = new UserModel()
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                PhoneNumber = "12345678",
                Email = "john.doe@example.com",
                Password = "Sh12345678#",
                UserName = "UserName"

            };
        }
        [TestMethod]
        public void TestToString()
        {
            var expectedString = "1, John, Doe, john.doe@example.com, 12345678, UserName";
            var actualString = user.ToString();
            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("S1234567812")]
        [DataRow("Sh12345678")]
        [DataRow("12345678#   ")]
        [DataRow("Sh12345678John")]
        [DataRow("S1234567812Doe")]
        [DataRow("Sh12345678Cameron")]
        public void Test_Exception_Password(string invalidPassword)
        {
            //Assert
            user.Password = invalidPassword;

            // Act
            Assert.ThrowsException<ArgumentException>(() => user.ValidatePassword());

        }

    }
}
