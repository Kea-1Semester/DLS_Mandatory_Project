using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClassLibrary
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
