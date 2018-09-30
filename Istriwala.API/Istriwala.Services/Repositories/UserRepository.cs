using Istriwala.Core.Interfaces;
using Istriwala.Core.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Istriwala.Services.Repositories
{
    public class UserRepository : IUserRepository
    {
        /// <summary>
        /// Create a user
        /// </summary>
        public User Create(User user)
        {
            user.Id = DataAccess.ExecuteSPNonQuery(DataAccess.ConnectionStrings.Istriwala, "CreateUser", user);
            return user;
        }

        /// <summary>
        /// Update a User
        /// </summary>
        public void Update(User user)
        {
            DataAccess.ExecuteSPNonQuery(DataAccess.ConnectionStrings.Istriwala, "CreateUser", user);
        }

        /// <summary>
        /// Get a User by id
        /// </summary>
        public User Get(int id)
        {
            return DataAccess.ExecuteSPGetItem<User>(DataAccess.ConnectionStrings.Istriwala, "GetUser", new { Id = id });
        }
    }
}