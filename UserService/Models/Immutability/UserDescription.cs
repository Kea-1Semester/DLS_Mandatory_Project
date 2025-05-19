using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using UserService.Interface;

namespace UserService.Models.CQRS
{
    public class UserDescription : UserModel
    {
        public UserDescription()
        {

        }
        
        #region Navigation Properties
        //public User User { get; set; }

        #region represent the alternate key
        public DateTime ModifiedDate { get; set; }
        [ForeignKey(nameof(User))]
        public int UserId { get; set; }
        #endregion

        #endregion

    }
}

