using Istriwala.Core.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Istriwala.Core.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Create a new User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        User Create(User user);

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user"></param>
        void Update(User user);

        /// <summary>
        /// Get a User by its Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User Get(int id);
       
    }
}
