using System;
using System.Collections.Generic;
using System.Text;
using Obiektuwa.Models;

namespace Obiektuwa.Classes {
    public class DataSeeder {
        private readonly Repository<MenuItem> _productRepo;
        private readonly Repository<Order> _orderRepo;
        private readonly Repository<Offer> _offerRepo;
        public DataSeeder(Repository<MenuItem> productRepo, Repository<Order> orderRepo, Repository<Offer> offerRepo) {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _offerRepo= offerRepo;
        }

        public void SeedData() 
        {
            SeedProduct();
            SeedOffer();
            SeedOrder();
        }

        private void SeedProduct() {
            if (_productRepo.GetAll().Count > 0) return;

            _productRepo.BulkAdd(new List<MenuItem>
            {
                new("Zupa dnia",                 5.99,  MenuItem.FoodCategory.Starter),
                new("Sałatka grecka",            8.99,  MenuItem.FoodCategory.Starter),
                new("Krewetki w czosnku",        14.99, MenuItem.FoodCategory.Starter),
                new("Bruschetta",                7.99,  MenuItem.FoodCategory.Starter),
                new("Talerz serów",              16.99, MenuItem.FoodCategory.Starter),
                new("Carpaccio wołowe",          13.99, MenuItem.FoodCategory.Starter),
                new("Zupa krem z dyni",          7.99,  MenuItem.FoodCategory.Starter),
                new("Nachos z guacamole",        9.99,  MenuItem.FoodCategory.Starter),
                new("Margherita",                12.99, MenuItem.FoodCategory.MainCourse),
                new("Pepperoni",                 14.99, MenuItem.FoodCategory.MainCourse),
                new("Pizza Hawajska",            14.49, MenuItem.FoodCategory.MainCourse),
                new("Pizza BBQ",                 15.99, MenuItem.FoodCategory.MainCourse),
                new("Pizza Wegetariańska",       13.49, MenuItem.FoodCategory.MainCourse),
                new("Burger wołowy",             16.99, MenuItem.FoodCategory.MainCourse),
                new("Burger drobiowy",           15.49, MenuItem.FoodCategory.MainCourse),
                new("Burger wegetariański",      14.99, MenuItem.FoodCategory.MainCourse),
                new("Makaron carbonara",         13.99, MenuItem.FoodCategory.MainCourse),
                new("Makaron bolognese",         13.49, MenuItem.FoodCategory.MainCourse),
                new("Makaron z owocami morza",   18.99, MenuItem.FoodCategory.MainCourse),
                new("Ryż z kurczakiem",          12.99, MenuItem.FoodCategory.MainCourse),
                new("Łosoś grillowany",          24.99, MenuItem.FoodCategory.MainCourse),
                new("Stek wołowy 200g",          34.99, MenuItem.FoodCategory.MainCourse),
                new("Kotlet schabowy",           15.99, MenuItem.FoodCategory.MainCourse),
                new("Pierogi ruskie",            11.99, MenuItem.FoodCategory.MainCourse),
                new("Pierogi z mięsem",          12.99, MenuItem.FoodCategory.MainCourse),
                new("Sernik",                    6.99,  MenuItem.FoodCategory.Dessert),
                new("Lody kulkowe",              5.99,  MenuItem.FoodCategory.Dessert),
                new("Tiramisu",                  8.99,  MenuItem.FoodCategory.Dessert),
                new("Brownie z lodami",          9.99,  MenuItem.FoodCategory.Dessert),
                new("Naleśniki z dżemem",        7.99,  MenuItem.FoodCategory.Dessert),
                new("Panna cotta",               7.49,  MenuItem.FoodCategory.Dessert),
                new("Szarlotka",                 6.99,  MenuItem.FoodCategory.Dessert),
                new("Cola 0.5L",                 4.99,  MenuItem.FoodCategory.Drink),
                new("Cola Zero 0.5L",            4.99,  MenuItem.FoodCategory.Drink),
                new("Woda niegazowana 0.5L",     2.99,  MenuItem.FoodCategory.Drink),
                new("Woda gazowana 0.5L",        2.99,  MenuItem.FoodCategory.Drink),
                new("Sok pomarańczowy",          4.49,  MenuItem.FoodCategory.Drink),
                new("Sok jabłkowy",              4.49,  MenuItem.FoodCategory.Drink),
                new("Piwo jasne 0.5L",           7.99,  MenuItem.FoodCategory.Drink),
                new("Piwo ciemne 0.5L",          8.49,  MenuItem.FoodCategory.Drink),
                new("Wino czerwone (kieliszek)", 12.99, MenuItem.FoodCategory.Drink),
                new("Wino białe (kieliszek)",    12.99, MenuItem.FoodCategory.Drink),
                new("Kawa czarna",               5.99,  MenuItem.FoodCategory.Drink),
                new("Kawa z mlekiem",            6.99,  MenuItem.FoodCategory.Drink),
                new("Herbata",                   4.99,  MenuItem.FoodCategory.Drink),
            });

            _productRepo.Save();
        }

        private void SeedOffer() {
            if (_offerRepo.GetAll().Count > 0) return;

            var allItems = _productRepo.GetAll();
            MenuItem Get(string name) => allItems.First(m => m.Name == name);

            _offerRepo.BulkAdd(new List<Offer>
            {
                new(new() { (Get("Margherita"), 2) },
                    (Offer.DiscountType.Percent, 10)),

                new(new() { (Get("Margherita"), 1), (Get("Cola 0.5L"), 1) },
                    (Offer.DiscountType.FixedAmount, 3.00)),

                new(new() { (Get("Burger wołowy"), 1), (Get("Cola 0.5L"), 1) },
                    (Offer.DiscountType.FixedPrice, 18.99)),

                new(new() { (Get("Margherita"), 2), (Get("Cola 0.5L"), 2) },
                    (Offer.DiscountType.Percent, 15)),

                new(new() { (Get("Stek wołowy 200g"), 1), (Get("Tiramisu"), 1) },
                    (Offer.DiscountType.Percent, 20)),

                new(new() { (Get("Sernik"), 1), (Get("Lody kulkowe"), 1) },
                    (Offer.DiscountType.FixedAmount, 5.00)),

                new(new() { (Get("Zupa dnia"), 1), (Get("Kotlet schabowy"), 1) },
                    (Offer.DiscountType.FixedPrice, 19.99)),

                new(new() { (Get("Piwo jasne 0.5L"), 2) },
                    (Offer.DiscountType.FixedAmount, 3.00)),
            });

            _offerRepo.Save();
        }

        private void SeedOrder() {
            if (_orderRepo.GetAll().Count > 0) return;

            var allItems = _productRepo.GetAll();
            MenuItem Get(string name) => allItems.First(m => m.Name == name);

            var order1 = new Order { Comment = "Bez cebuli na burgerze", IsTakeaway = false };
            order1 = order1 + Get("Margherita");
            order1 = order1 + Get("Margherita");
            order1 = order1 + Get("Burger wołowy");
            order1 = order1 + Get("Cola 0.5L");
            order1 = order1 + Get("Cola 0.5L");
            order1 = order1 + Get("Woda niegazowana 0.5L");
            order1 = order1 + Get("Tiramisu");
            order1 = order1 + Get("Sernik");

            var order2 = new Order { IsTakeaway = true };
            order2 = order2 + Get("Pepperoni");
            order2 = order2 + Get("Cola 0.5L");
            order2 = order2 + Get("Lody kulkowe");
            order2.ChangeState(Order.OrderState.finished);

            var order3 = new Order { Comment = "Faktura na firmę", IsTakeaway = false };
            order3 = order3 + Get("Zupa dnia");
            order3 = order3 + Get("Zupa dnia");
            order3 = order3 + Get("Stek wołowy 200g");
            order3 = order3 + Get("Łosoś grillowany");
            order3 = order3 + Get("Wino czerwone (kieliszek)");
            order3 = order3 + Get("Wino białe (kieliszek)");
            order3 = order3 + Get("Kawa czarna");
            order3 = order3 + Get("Kawa czarna");
            order3.ChangeState(Order.OrderState.finished);

            var order4 = new Order { Comment = "Osobne rachunki poprosimy", IsTakeaway = false };
            order4 = order4 + Get("Pizza Hawajska");
            order4 = order4 + Get("Pizza BBQ");
            order4 = order4 + Get("Piwo jasne 0.5L");
            order4 = order4 + Get("Piwo jasne 0.5L");
            order4 = order4 + Get("Piwo ciemne 0.5L");

            var order5 = new Order { Comment = "Klient wyszedł", IsTakeaway = false };
            order5 = order5 + Get("Makaron carbonara");
            order5 = order5 + Get("Sok pomarańczowy");
            order5.ChangeState(Order.OrderState.canceled);

            _orderRepo.BulkAdd(new List<Order> { order1, order2, order3, order4, order5 });
            _orderRepo.Save();
        }
    }
}
