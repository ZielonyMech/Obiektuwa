using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using Obiektuwa.Classes;

namespace Obiektuwa.Models {
    public class Offer {
        public enum DiscountType {
            FixedPrice,
            Percent,
            FixedAmount
        }
        
        public List<(MenuItem item, uint quantity)> RequiredPositions { get; init; }
        public (DiscountType type, double value) Discout;
        
        public Offer(List<(MenuItem, uint)> requiredPositions, (DiscountType type, double value) discount) {
            RequiredPositions = requiredPositions;
            Discout = discount;
        }

        public double GetOfferBasePrice() {
            double basePrice = 0.0;
            
            foreach (var (item, quantity) in RequiredPositions) {
                basePrice += item.Price * quantity;
            }

            return basePrice;
        }

        public double GetFinalPrice() {
            double baseOfferPrice = GetOfferBasePrice();
            IDiscountStrategy strategy = DiscountStrategyFactory.Create(Discout.type);

            return strategy.CalculateFinalPrice(baseOfferPrice, Discout.value);
        }

        public double GetDiscountAmount() {
            return Math.Round(GetOfferBasePrice() - GetFinalPrice(), 2);
        }

        public override string ToString() {
            var sb = new StringBuilder();
            string discountDescription = Discout.type switch {
                DiscountType.FixedPrice => $"cena zestawu {Discout.value:f2} zł",
                DiscountType.Percent => $"rabat {Discout.value:f0}%",
                DiscountType.FixedAmount => $"rabat {Discout.value:f2} zł",
                _ => "brak rabatu"
            };

            sb.AppendLine($"Oferta: {discountDescription}");
            sb.AppendLine("Wymagane pozycje:");

            foreach (var (item, quantity) in RequiredPositions) {
                sb.AppendLine($"- {quantity} x {item.Name} ({item.Price:f2} zł)");
            }

            sb.AppendLine($"Cena bazowa: {GetOfferBasePrice():f2} zł");
            sb.AppendLine($"Cena po rabacie: {GetFinalPrice():f2} zł");
            sb.Append($"Oszczędzasz: {GetDiscountAmount():f2} zł");

            return sb.ToString();
        }
    }
}
