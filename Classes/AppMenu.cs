using System;
using Microsoft.Extensions.DependencyInjection;
using Obiektuwa.Models;
using Obiektuwa.Classes;

namespace Obiektuwa
{
    public class AppMenu
    {
        private readonly Repository<MenuItem> _productRepo;
        private readonly Repository<Order> _orderRepo;
        private readonly Func<Order> _orderFactory;
        private readonly OfferManager _offerManager;
        public AppMenu(Repository<MenuItem> productRepo, Repository<Order> orderRepo, OfferManager offerManager, Func<Order> orderFactory)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _orderFactory = orderFactory;
            _offerManager = offerManager;
        }

        public void Run()
        {
            bool isRunning = true;

            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== SYSTEM OBSŁUGI ZAMÓWIEŃ ===");
                Console.WriteLine("1. Złóż nowe zamówienie");
                Console.WriteLine("2. Pokaż aktywne zamówienia");
                Console.WriteLine("3. Zobacz Menu");
                Console.WriteLine("0. Wyjdź");
                Console.Write("\nWybierz opcję: ");

                string choice = Console.ReadLine()?.Trim() ?? "";

                switch (choice)
                {
                    case "1":
                        CreateNewOrder();
                        break;
                    case "2":
                        ShowActiveOrders();
                        break;
                    case "3":
                        ShowProducts();
                        break;
                    case "0":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Niepoprawna opcja. Wybierz opcję spoza 0 - 3");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private void DisplayMenuItems(List<MenuItem> menuItems) {

            for (int i = 0; i < menuItems.Count; i++) {
                Console.WriteLine($"{i + 1}. {menuItems[i].Name} - {menuItems[i].Price:f2} zł");
            }
        }

        private void CreateNewOrder()
        {
            var menuItems = _productRepo.GetAll();

            if (menuItems.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("Menu jest puste :/");
                Console.WriteLine("Wciśnij enter....");
                Console.ReadKey();
                return;
            }


            var newOrder = _orderFactory();
            bool isOrdering = true;

            while (isOrdering)
            {
                Console.Clear();
                Console.WriteLine("=== TWORZENIE ZAMÓWIENIA ===");
                Console.WriteLine("\nMenu:\n");

                DisplayMenuItems(menuItems);

                Console.WriteLine($"\nAktualna kwota: {newOrder.CalculateFinalPrice():f2} zł");
                Console.WriteLine("Wpisz numer dania, aby go dodać (lub '0' aby zakończyć):");

                string input = Console.ReadLine()?.Trim() ?? "";

                if (input == "0")
                {
                    isOrdering = false;
                    continue;
                }
                if (int.TryParse(input, out int selectedIndex) && selectedIndex > 0 && selectedIndex <= menuItems.Count)
                {
                    var product = menuItems[selectedIndex - 1];
                    newOrder += product;
                    Console.WriteLine($"\nDodano: {product.Name}!");
                    Console.WriteLine("Wciśnij enter....");
                    Console.ReadKey();
                }
                else
                {
                    Console.WriteLine("\nNie ma takiego dania w menu! Spróbuj ponownie.");
                    Console.ReadKey();
                }
            }

            if (newOrder.Positions.Count > 0)
            {
                
                Console.WriteLine("Wpisz treść komentarza do zamówienia i wciśnij Enter (lub zostaw puste, aby pominąć):");
                string commentInput = Console.ReadLine()?.Trim() ?? "";

                if (!string.IsNullOrEmpty(commentInput))
                {
                    newOrder.Comment = commentInput;
                }

                Console.WriteLine("\nCzy zamówienie jest na wynos? (T/N):");
                string takeawayInput = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (takeawayInput == "T" || takeawayInput == "TAK")
                {
                    newOrder.IsTakeaway = true;
                }

                Console.WriteLine("\nZapisywanie zamówienia....");
                _orderRepo.Add(newOrder);
                _orderRepo.Save();
                Console.WriteLine("Zamówienie przyjęte! Wciśnij enter aby zakończyć....");
            }
            else
            {
                Console.WriteLine("\nAnulowano puste zamówienie. Wciśnij enter aby zakończyć...");
            }
            Console.ReadKey();
        }

        private void ShowActiveOrders()
        {
            Console.Clear();
            Console.WriteLine("=== AKTYWNE ZAMÓWIENIA ===");
            var activeOrders = _orderRepo.FindAll(elem => elem.State == Order.OrderState.inProgress);

            if (activeOrders.Count == 0)
            {
                Console.WriteLine("Brak zamówień w trakcie realizacji.");
                Console.WriteLine("\nWciśnij enter....");
                Console.ReadKey();
                return;
            }
            
            
            foreach (var order in activeOrders)
            {
                Console.WriteLine("\n--------------------------");
                Console.WriteLine(order);
                var price = _offerManager.CalculateFinalPriceWithDiscounts(order.Positions);

                Console.WriteLine($"Cena łączna {price.finalPrice:f2}, łączne rabaty {price.discount:f2}");
            }
           
            Console.WriteLine("\nWciśnij enter....");
            Console.ReadKey();
        }

        private void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine("=== NASZE MENU ===");
            var items = _productRepo.GetAll();
            foreach (var item in items)
            {
                Console.WriteLine(item);
            }
            Console.WriteLine("\nWciśnij enter aby wrócić....");
            Console.ReadKey();
        }
    }
}