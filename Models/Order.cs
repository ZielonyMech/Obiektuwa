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
        public string comment { get; init; }
        public bool isTakeaway { get; set; }
        private OrderState state { get; set; }

        public List<Product> positions { get; set; } = new List<Product>();


        public void ChangeState() {
            if (state == OrderState.inProgress) {
                state = OrderState.finished;
            }
        }

        public int CalculateFinalPrice() {
            if (positions == null) {
                return 0; 
            }
            return (int)positions.Sum(p => p.productPrice);
        }
  

    

        
    }
}
