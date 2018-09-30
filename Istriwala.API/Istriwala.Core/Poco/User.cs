using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Istriwala.Core.Constants;

namespace Istriwala.Core.Poco
{
    public class User
    {
    
        #region Constructors

        public User() { }

        #endregion Constructors

        #region Public Properties

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Gender { get; set; }
        public string ProfileUrl { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        public List<Roles> Roles { get; set; } 

        #endregion Public Properties
    }
}
