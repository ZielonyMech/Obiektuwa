using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Obiektuwa.Models
{
    public class Order
    {
        public enum OrderState
        {
            inProgress,
            finished,
            canceled,
            none
        }

        public Guid ID { get; init; } = Guid.NewGuid();
        public string? Comment { get; set; }
        public bool IsTakeaway { get; set; } = false;
        public bool HasStudentDiscount { get; set; } = false;
        public OrderState State { get; set; } = OrderState.inProgress;
        public List<(MenuItem Item, uint Quantity)> Positions { get; } = new List<(MenuItem, uint)>();

        public void ChangeState(OrderState newState)
        {
            if (State == OrderState.finished && newState == OrderState.inProgress)
            {
                throw new InvalidOperationException("Te zamówienie zostało już zakończone!");
            }
            State = newState;
        }

        public static Order operator +(Order? order, MenuItem? item)
        {
            if (order is null || item is null)
            {
                throw new ArgumentException("Któryś z parametrów jest null!");
            }
            if (order.Positions.Any(p => p.Item.ID == item.ID))
            {
                var existingPosition = order.Positions.First(p => p.Item.ID == item.ID);
                order.Positions.Remove(existingPosition);
                order.Positions.Add((item, existingPosition.Quantity + 1));
            }
            else
            {
                order.Positions.Add((item, 1));
            }

            return order;
        }

        public static Order operator -(Order? order, MenuItem? item)
        {
            if (order is null || item is null)
            {
                throw new ArgumentException("Któryś z parametrów jest null!");
            }
            if (!order.Positions.Any(p => p.Item.ID == item.ID))
            {
                throw new InvalidOperationException("Nie można usunąć pozycji, która nie istnieje w zamówieniu!");
            }

            var existingPosition = order.Positions.First(p => p.Item.ID == item.ID);

            if (existingPosition.Quantity == 1)
            {
                order.Positions.Remove(existingPosition);
            }
            else
            {
                order.Positions.Remove(existingPosition);
                order.Positions.Add((item, existingPosition.Quantity - 1));
            }

            return order;
        }

        public override string ToString() 
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Zamówienie ID: {ID}");
            sb.AppendLine($"Komentarz: {Comment}");
            sb.AppendLine($"Na wynos: {(IsTakeaway ? "Tak" : "Nie")}");
            sb.AppendLine($"Zniżka studencka: {(HasStudentDiscount ? "Tak" : "Nie")}");
            sb.AppendLine($"Stan zamówienia: {State}");
            sb.AppendLine($"{"Ilość",-10} | Pozycja");
            sb.AppendLine(new string('-', 40));

            foreach (var (item, quantity) in Positions) {
                sb.AppendLine($"{quantity,-10} | {item}");
            }

            return sb.ToString();
        }

        public double CalculateFinalPrice() 
        {
            return Positions.Sum(p => p.Item.Price * p.Quantity);
        }
    }
}
