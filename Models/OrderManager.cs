using System;
using System.Collections.Generic;
using System.Text;
using Obiektuwa.Classes;

namespace Obiektuwa.Models
{
    public class OrderManager : Repository<Order>
    {
        public List<Order> GetActiveOrders() {
            return FindAll(order => order.State == Order.OrderState.inProgress);
        }
    }
}
