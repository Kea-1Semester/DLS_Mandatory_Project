using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserClassLibrary
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
