using Istriwala.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Istriwala.Core.Poco;
using System.Collections.Specialized;
using System.Data;

namespace Istriwala.Services.Repositories
{
    public class AuthRepository: IAuthRepository
    {
      
        public User Login(string uid, string pwd)
        {
            User user = null;

            var parms = new ListDictionary
            {
                { "UserName", uid },
                { "Password", pwd }
            };
            DataSet dsLogin = DataAccess.ExecuteSPSelect(DataAccess.ConnectionStrings.Istriwala, "Login", parms);

            if (dsLogin.Tables.Count > 0)
            {
                if (dsLogin.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = dsLogin.Tables[0].Rows[0];
                    user = new User
                    {
                        Id = dr.Field<int>("Id"),
                        UserName = dr.Field<string>("UserName"),
                        EmailId = dr.Field<string>("EmailId"),
                        Name = dr.Field<string>("Name").Trim()
                    };

                    if (dsLogin.Tables[1].Rows.Count > 0)
                    {
                        user.Roles = new List<Istriwala.Core.Constants.Roles>();

                        foreach (DataRow row in dsLogin.Tables[1].Rows)
                        {
                            user.Roles.Add(row.Field<Istriwala.Core.Constants.Roles>("RoleId"));
                        }
                    }
                }
            }

            return user;
        }
    }
}
