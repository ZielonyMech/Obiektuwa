using System;
using System.Collections.Generic;
using System.Text;

namespace Obiektuwa.Models
{
    public class Order
    {
        public enum OrderState{
            inProgress,
            finished,
            canceled
        }


        public int id;
        public string? Comment { get; init; }
        public bool IsTakeaway { get; set; }
        private OrderState state { get; set; }

        public List<MenuItem> Positions { get; set; } = new List<MenuItem>();


        public void ChangeState() {
            if (state == OrderState.inProgress) {
                state = OrderState.finished;
            }
        }

        public int CalculateFinalPrice() {
            if (Positions == null) {
                return 0; 
            }
            return (int)Positions.Sum(p => p.Price);
        }

        public static Order? operator +(Order? order, MenuItem? item) {
            if (order != null && item != null) { 
                order.Positions.Add(item);
            }
            return order;
        }

        public static Order? operator -(Order? order, MenuItem? item) { 
            if(order != null && item != null)
            {
                order.Positions.Remove(item);
            }
            return order;
        }
        
    }
}
