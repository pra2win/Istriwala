using Dapper;
using Istriwala.Core.Interfaces;
using Istriwala.Core.Poco;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
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
            ListDictionary parms = new ListDictionary();
            parms.Add("@Id", 0);
            parms.Add("@UserName", user.UserName);
            parms.Add("@Address", user.Address);
            parms.Add("@EmailId", user.EmailId);
            parms.Add("@Gender", user.Gender);
            parms.Add("@MobileNo", user.MobileNo);
            parms.Add("@Name", user.Name);
            parms.Add("@Password", user.Password);
            parms.Add("@ProfileUrl", user.ProfileUrl);
            parms.Add("@Roles", string.Join(",", user.Roles.Select(u => (int)u)));

            user.Id = DataAccess.ExecuteSPNonQuery(DataAccess.ConnectionStrings.Istriwala, "CreateUser", parms);
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
            //return DataAccess.ExecuteSPGetItem<User>(DataAccess.ConnectionStrings.Istriwala, "GetUser", new { Id = id });
            User user = new User();
            try
            {
                using (var sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["Istriwala"].ConnectionString))
                {
                    sqlConnection.Open();
                    using (var result = sqlConnection.QueryMultiple("GetUser", param: new { Id = id }, commandType: CommandType.StoredProcedure))
                    {
                        if (result != null)
                        {
                            user = result.Read<User>().First();
                            user.Roles = result.Read<Core.Constants.Roles>().ToList();
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return user;
        }
    }
}