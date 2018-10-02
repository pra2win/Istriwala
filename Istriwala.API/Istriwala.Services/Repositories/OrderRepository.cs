using Istriwala.Core.Interfaces;
using Istriwala.Core.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Istriwala.Services.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        /// <summary>
        /// Place new order for clients
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public Order PlaceOrder(Order order)
        {
            var orderDetails = DataAccess.ExecuteSPGetItem<Order>(DataAccess.ConnectionStrings.Istriwala, "PlaceOrder", order);
            return orderDetails;
        }

        /// <summary>
        /// Get Order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Order GetOrder(int id)
        {
            return DataAccess.ExecuteSPGetItem<Order>(DataAccess.ConnectionStrings.Istriwala, "GetOrder", new { OrderId = id });
        }

        public List<Order> GetUserOrders(int id)
        {
            return DataAccess.ExecuteSPGetList<Order>(DataAccess.ConnectionStrings.Istriwala, "GetUserOrders", new { UserId = id });
        }

    }
}
