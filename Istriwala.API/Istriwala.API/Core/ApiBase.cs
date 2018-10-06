using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Http;

namespace Istriwala.API.Core
{
    /// <summary>
    /// Custom base class for all controllers
    /// </summary>
    public class ApiBase : ApiController
    {
        public int UserId
        {
            get
            {
                var identity = User.Identity as ClaimsIdentity;
                var userClaim = identity.Claims.Where(i => i.Type == "userId").First();
                int userId = Int32.Parse(userClaim.Value);
                return userId;
            }
        }
    }
}