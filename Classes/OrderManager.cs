using System;
using System.Collections.Generic;
using System.Text;
using Obiektuwa.Models;

namespace Obiektuwa.Classes {
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

            if (applicableOffers.Count > 0) {
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
