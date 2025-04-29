using System.Text.RegularExpressions;

namespace UserService.Models.DTO
{
    public class UserInfo : UserModel
    {
        public Guid Guid { get; set; }
        public long LastModifiedTicks { get; set; }
        public UserRole Role { get; set; }


        public UserInfo()
        {

        }
    }
}
public enum UserRole
{
    User
}