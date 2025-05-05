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
    }
}

public record UserUpdat 
{

}


