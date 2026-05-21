using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Obiektuwa.Models {
    internal class Offer {
        public enum DiscountType {
            FixedPrice,
            Percent,
            FixedAmount
        }
        
        public List<(MenuItem item, uint quantity)> RequiredPositions { get; init; }
        public (DiscountType type, double value) Discout;
        
        public Offer(List<(MenuItem, uint)> requiredPositions) {
            RequiredPositions = requiredPositions;
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

            return (Discout.type) switch {
                DiscountType.FixedPrice => Discout.value,
                DiscountType.Percent => baseOfferPrice - (baseOfferPrice * Discout.value / 100),
                DiscountType.FixedAmount => baseOfferPrice - Discout.value,
                _ => baseOfferPrice
            };
        }
    }
}
