using Microsoft.Extensions.DependencyInjection;
using Obiektuwa.Classes;
using Obiektuwa.Models;
using System;

namespace Obiektuwa {
    internal class Program {
        static ServiceProvider BootstratpDI() {
            return new ServiceCollection()
                .AddSingleton<Repository<MenuItem>>()
                .AddSingleton<OfferManager>()
                .AddSingleton<OrderManager>()
                .AddSingleton<AppMenu>()
                .AddTransient<Order>()
                .BuildServiceProvider();
        }

        static void Main(string[] args) { 
            var services = BootstratpDI();

            var productRepo = services.GetRequiredService<Repository<MenuItem>>();
            if (productRepo.FindAll(x => true).Count == 0)
            {
                MenuItem pizza = new("Pizza", 25.90, MenuItem.FoodCategory.MainCourse);
                MenuItem burger = new("Burger", 15.50, MenuItem.FoodCategory.MainCourse);
                productRepo.BulkAdd(new() { pizza, burger });
                productRepo.Save();

                var offerManager = services.GetRequiredService<OfferManager>();
                Offer pizzaOffer = new(new() { (pizza, 2) }, (Offer.DiscountType.FixedPrice, 35.00));
                offerManager.Add(pizzaOffer);
                offerManager.Save();
            }
            var app = services.GetRequiredService<AppMenu>();
            app.Run();

        }
    }   
}