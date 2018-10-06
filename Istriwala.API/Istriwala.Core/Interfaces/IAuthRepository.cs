using Istriwala.Core.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Istriwala.Core.Interfaces
{
    public interface IAuthRepository
    {
        User Login(string UserName, string Password);
    }
}
