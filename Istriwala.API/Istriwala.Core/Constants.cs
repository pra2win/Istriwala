using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Istriwala.Core
{
    public class Constants
    {
        /// <summary>
        /// Must be sync with dbo.Role table
        /// </summary>
        public enum Roles
        {
            Client = 1,
            Admin = 2,
            SuperAdmin = 3
        }

        /// <summary>
        /// must be sync with ProductType table
        /// </summary>
        public enum ProductTypes
        {
            Male = 1,
            Female = 2
        }
    }
}
