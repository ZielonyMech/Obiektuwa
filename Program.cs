using Microsoft.Extensions.DependencyInjection;
using Obiektuwa.Classes;
using Obiektuwa.Models;

namespace Obiektuwa {
    internal class Program {
        static ServiceProvider BootstratpDI() {
            return new ServiceCollection()
                .AddSingleton<Repository<MenuItem>>()
                .BuildServiceProvider();
        }

        static void Main(string[] args) {
            MenuItem product = new("Pizza", 25.90, MenuItem.FoodCategory.MainCourse);
            MenuItem product1 = new("Burger", 15.50, MenuItem.FoodCategory.MainCourse);

            var services = BootstratpDI();
            var productRepo = services.GetRequiredService<Repository<MenuItem>>();

            productRepo.BulkAdd(new() { product, product1 });
            productRepo.Save();
        }
    }   
}
