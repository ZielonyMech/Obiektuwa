using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;

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

            return (Discout.type) switch {
                DiscountType.FixedPrice => Discout.value,
                DiscountType.Percent => baseOfferPrice - (baseOfferPrice * Discout.value / 100),
                DiscountType.FixedAmount => baseOfferPrice - Discout.value,
                _ => baseOfferPrice
            };
        }

        public double GetDiscountAmount() {
            return Math.Round(GetOfferBasePrice() - GetFinalPrice(), 2);
        }
    }

    public class OfferManager {
        public List<Offer> Offers { get; init; } = new();
    
        public OfferManager(List<Offer> Offers) {
            this.Offers = Offers;
        }

        public OfferManager() { }

        public (double finalPrice, double discount) CalculateFinalPriceWithDiscounts(List<(MenuItem Item, uint Quantity)> orderPositions) {
            double finalPrice = 0.0;
            double discount = 0.0;  

            foreach (var (item, quantity) in orderPositions) {
                var offer = Offers.Find(offer => offer.RequiredPositions.Exists(pos => pos.item.ID == item.ID &&
                                                                    quantity >= pos.quantity));
                if (offer is null) continue;

                finalPrice += offer.GetFinalPrice();
                discount += offer.GetDiscountAmount();
            }

            return (finalPrice, discount);
        }

        public void AddOffer(Offer offer) {
            Offers.Add(offer);
        }
    }
}
