using System.Text.Json.Serialization;

namespace UserService.Models.DTO
{
    public class UserInfo : UserModel
    {
        public Guid Guid { get; set; }
        public long LastModifiedTicks { get; set; }

    
    
        public UserInfo()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfo"/> class with the specified email and password.
        /// Use this constructor when only authentication information is required, such as during user login.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password (should be hashed in production).</param>

        public UserInfo(string email, string password) : base(email, password)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInfo"/> class with the specified user details.
        /// Use this constructor when creating a user with full profile information.
        /// </summary>
        /// <param name="firstName">The user's first name.</param>
        /// <param name="lastName">The user's last name.</param>
        /// <param name="email">The user's email address.</param>
        /// <param name="phoneNumber">The user's phone number.</param>
        /// <param name="userName">The user's username for login.</param>
        /// <param name="password">The user's password (should be hashed in production).</param>

        public UserInfo(string firstName, string lastName, string email, string phoneNumber, string userName, string password, long lastModifiedTicks) : base(firstName, lastName, email, phoneNumber, userName, password)
        {
            LastModifiedTicks = lastModifiedTicks;
        }

    }
}

public record UserLoginInfo 
{
    public string Email { get; set; }
    public string Password { get; set; }

}

public record UserRegisterInfo
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}


