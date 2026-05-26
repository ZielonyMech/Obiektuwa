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


        public Guid ID { get;  } = Guid.NewGuid();
        public string? Comment { get; init; }
        public bool IsTakeaway { get; set; }
        public OrderState State { get; private set; } = OrderState.inProgress;

        public List<MenuItem> Positions { get; } = new List<MenuItem>();


        public void ChangeState(OrderState newState) {
            if (State == OrderState.finished && newState == OrderState.inProgress) {
                throw new InvalidOperationException("Te zamówienie zostało już zakończone!");
            
            }
            State = newState;
        }

        public double CalculateFinalPrice() {
            if (Positions == null) {
                return 0; 
            }
            return Positions.Sum(p => p.Price);
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
