using Istriwala.Core.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Istriwala.Core.Interfaces
{
    public interface IOrderRepository
    {
        /// <summary>
        /// Please a new order for client
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        Order PlaceOrder(Order order);
    }
}
