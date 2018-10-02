using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Istriwala.Core.Poco
{
    public class Order
    {

        #region Constructors
        public Order()
        {

        }
        #endregion Constructors

        #region Public Properties
        public int OrderId { get; set; }
        public int OrderNo { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime OrderUpdateDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Discount { get; set; }
        public decimal NetAmount { get; set; }
        public string OrderLatitude { get; set; }
        public string OrderLongtitude { get; set; }
        public bool IsCancled { get; set; }
        public bool IsBilled { get; set; }

        #endregion Public Properties
    }
}
