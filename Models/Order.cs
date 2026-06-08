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
            canceled
        }

        //private readonly OfferManager _offerManager;
        public Guid ID { get; init; } = Guid.NewGuid();
        public string? Comment { get; set; }
        public bool IsTakeaway { get; set; } = false;
        public OrderState State { get; private set; } = OrderState.inProgress;
        public List<(MenuItem Item, uint Quantity)> Positions { get; } = new List<(MenuItem, uint)>();

        //public Order(OfferManager offerManager) { 
        //
        //    _offerManager = offerManager;
        //}
        
        public void ChangeState(OrderState newState)
        {
            if (State == OrderState.finished && newState == OrderState.inProgress)
            {
                throw new InvalidOperationException("Te zamówienie zostało już zakończone!");
            }
            State = newState;
        }

        public double CalculateFinalPrice(OfferManager offerManager)
        {
            if (Positions == null || Positions.Count == 0)
            {
                return 0;
            }

            var discounts = offerManager.CalculateFinalPriceWithDiscounts(Positions).discount;

            return Positions.Sum(p => p.Item.Price * p.Quantity) - discounts;
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

        public void DisplayOrder(OfferManager offerManager)
        {
            Console.WriteLine($"Zamówienie ID: {ID}");
            Console.WriteLine($"Komentarz: {Comment}");
            Console.WriteLine($"Na wynos: {(IsTakeaway ? "Tak" : "Nie")}");
            Console.WriteLine($"Stan zamówienia: {State}");
            Console.WriteLine("Ilośc pozycji | Pozycja:");

            foreach (var (item, quantity) in Positions)
            {
                Console.WriteLine($"{quantity} | {item}");
            }
            Console.WriteLine($"Zniżki: {offerManager.CalculateFinalPriceWithDiscounts(Positions).discount:f2} zł");
            Console.WriteLine($"Łączna cena: {CalculateFinalPrice(offerManager):f2} zł");
        }
    }
}