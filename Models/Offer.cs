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

        private readonly Repository<Offer> _offerRepository;

        public OfferManager(Repository<Offer> offerRepository) {
            _offerRepository = offerRepository;
        }

        public (double finalPrice, double discount) CalculateFinalPriceWithDiscounts(List<(MenuItem Item, uint Quantity)> orderPositions) {
            double finalPrice = 0.0;
            double discount = 0.0;

            List<Offer> applicableOffers = new List<Offer>();

            foreach (var offer in _offerRepository.GetAll()) {
                bool isApplicable = offer.RequiredPositions
                    .All(required => orderPositions.Any(order => order.Item.ID == required.item.ID));

                if (isApplicable) applicableOffers.Add(offer);
            }

            if (applicableOffers.Count > 0) 
            { 
                applicableOffers = applicableOffers
                .OrderByDescending(offer => offer.GetDiscountAmount())
                .ToList();
            }

            var remaining = orderPositions.ToDictionary(p => p.Item.ID, p => p.Quantity);

            foreach (var offer in applicableOffers) {
                while (IsOfferApplicable(offer, remaining)) {
                    finalPrice += offer.GetFinalPrice();
                    discount += offer.GetDiscountAmount();
                    DeductUsedItems(offer, remaining);
                }
            }

            finalPrice += remaining.Sum(elem => {
                var item = orderPositions.First(p => p.Item.ID == elem.Key).Item;
                return item.Price * elem.Value;
            });

            return (finalPrice, discount);
        }

        private bool IsOfferApplicable(Offer offer, Dictionary<Guid, uint> remaining) {
            return offer.RequiredPositions.All(pos =>
                remaining.TryGetValue(pos.item.ID, out var qty) && qty >= pos.quantity);
        }

        private void DeductUsedItems(Offer offer, Dictionary<Guid, uint> remaining) {
            foreach (var pos in offer.RequiredPositions) {
                remaining[pos.item.ID] -= pos.quantity;
            }
        }
    }
}
