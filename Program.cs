using Microsoft.Extensions.DependencyInjection;
using Obiektuwa.Classes;
using Obiektuwa.Models;
using System;

namespace Obiektuwa {
    internal class Program {
        static ServiceProvider BootstratpDI() {
            return new ServiceCollection()
                .AddSingleton<Repository<MenuItem>>()
                .AddSingleton<Repository<Offer>>()
                .AddSingleton<Repository<Order>>()

                .AddSingleton<OfferManager>()
                .AddSingleton<AppMenu>()
                .AddTransient<Order>()
                .AddSingleton<Func<Order>>(provider => () => provider.GetRequiredService<Order>())
                .BuildServiceProvider();
        }

        static void Main(string[] args) { 
            var services = BootstratpDI();

            var productRepo = services.GetRequiredService<Repository<MenuItem>>();
            if (productRepo.GetAll().Count == 0)
            {
                MenuItem pizza = new("Pizza", 25.90, MenuItem.FoodCategory.MainCourse);
                MenuItem burger = new("Burger", 15.50, MenuItem.FoodCategory.MainCourse);
                productRepo.BulkAdd(new() { pizza, burger });
                productRepo.Save();

                var offers = services.GetRequiredService<Repository<Offer>>();
                Offer pizzaOffer = new(new() { (pizza, 2) }, (Offer.DiscountType.FixedPrice, 35.00));
                Offer burgerPizzaOffer = new(new() { (pizza, 1), (burger, 1) }, (Offer.DiscountType.FixedPrice, 1.00));
                offers.Add(pizzaOffer);
                offers.Add(burgerPizzaOffer);
                offers.Save();
            }

            var app = services.GetRequiredService<AppMenu>();
            app.Run();
        }
    }   
}