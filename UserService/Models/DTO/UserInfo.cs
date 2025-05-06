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

        public UserInfo(string email, string password) : base(email, password)
        {
        }
    }
}

public record UserLoginInfo 
{
    public string Email { get; set; }
    public string Password { get; set; }

}


