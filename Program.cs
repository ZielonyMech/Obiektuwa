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

                .AddSingleton<DataSeeder>()
                .AddSingleton<OfferManager>()
                .AddSingleton<AppMenu>()

                .BuildServiceProvider();
        }

        static void Main(string[] args) { 
            var services = BootstratpDI();

            var seeder = services.GetRequiredService<DataSeeder>();
            seeder.SeedData();

            var app = services.GetRequiredService<AppMenu>();
            app.Run();
        }
    }   
}