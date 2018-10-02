using Istriwala.Core.Interfaces;
using Istriwala.Core.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Istriwala.API.Controllers
{
    [RoutePrefix("api/Order")]
    public class OrderController : ApiController
    {

        #region Fields

        IOrderRepository _repository;

        #endregion Fields

        #region Public Methods
        public OrderController(IOrderRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Place order for user
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public IHttpActionResult PlaceOrder(Order order)
        {
            var result = _repository.PlaceOrder(order);
            return Ok(result);
        }

        #endregion  Public Methods
    }
}
