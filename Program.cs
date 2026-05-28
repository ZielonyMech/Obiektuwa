using Microsoft.Extensions.DependencyInjection;
using Obiektuwa.Classes;
using Obiektuwa.Models;

namespace Obiektuwa {
    internal class Program {
        static ServiceProvider BootstratpDI() {
            return new ServiceCollection()
                .AddSingleton<Repository<MenuItem>>()
                .AddSingleton<OfferManager>()
                .AddTransient<Order>()
                .BuildServiceProvider();
        }

        static void Main(string[] args) {
            MenuItem product = new("Pizza", 25.90, MenuItem.FoodCategory.MainCourse);
            MenuItem product1 = new("Burger", 15.50, MenuItem.FoodCategory.MainCourse);
            
            var services = BootstratpDI();

            var offerManager = services.GetRequiredService<OfferManager>();
            Offer pizzaOffer = new(new() { (product, 2) }, (Offer.DiscountType.FixedPrice, 35.00));
            offerManager.AddOffer(pizzaOffer);

            var productRepo = services.GetRequiredService<Repository<MenuItem>>();

            productRepo.BulkAdd(new() { product, product1 });
            productRepo.Save();

            var order1 = services.GetRequiredService<Order>();

            order1 += product;
            order1 += product1;
            order1 += product;
            order1 += product1;
            order1 += product1;

            order1.DisplayOrder();
        }
    }   
}
