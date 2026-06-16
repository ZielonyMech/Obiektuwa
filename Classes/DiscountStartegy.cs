using System;
using System.Collections.Generic;
using System.Text;
using Obiektuwa.Models;

namespace Obiektuwa.Classes {
    public interface IDiscountStrategy 
    {
        double CalculateFinalPrice(double basePrice, double discountValue);
    }

    public static class DiscountStrategyFactory {
        public static IDiscountStrategy Create(Offer.DiscountType discountType) {
            return discountType switch {
                Offer.DiscountType.FixedPrice => new FixedPriceDiscountStrategy(),
                Offer.DiscountType.Percent => new PercentDiscountStrategy(),
                Offer.DiscountType.FixedAmount => new FixedAmountDiscountStrategy(),
                _ => throw new ArgumentOutOfRangeException(nameof(discountType), discountType, "Nieobsługiwany typ rabatu")
            };
        }
    }

    public class PercentDiscountStrategy : IDiscountStrategy 
    {
        public double CalculateFinalPrice(double basePrice, double discountValue) 
        {
            return basePrice - (basePrice * discountValue / 100);
        }
    }

    public class FixedPriceDiscountStrategy : IDiscountStrategy 
    {
        public double CalculateFinalPrice(double basePrice, double discountValue) {
            return discountValue;
        }
    }

    public class FixedAmountDiscountStrategy : IDiscountStrategy {
        public double CalculateFinalPrice(double basePrice, double discountValue) {
            return basePrice - discountValue;
        }
    }
}
