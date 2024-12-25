using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EComercial.Enum;

namespace EComercial.Core
{
    internal class Order
    {
        public int Id { get; set; }
        public int CostumerId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatusEnum OrderStatus { get; set; }
        public List<OrderItem> Items { get; set; }

    }
}
