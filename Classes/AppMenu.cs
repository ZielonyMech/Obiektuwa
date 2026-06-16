using System;
using Microsoft.Extensions.DependencyInjection;
using Obiektuwa.Models;
using Obiektuwa.Classes;
using Configurator.UI;

namespace Obiektuwa
{
    public class AppMenu
    {
        private readonly Repository<MenuItem> _productRepo;
        private readonly Repository<Order> _orderRepo;
        private readonly Repository<Offer> _offerRepo;
        private readonly OfferManager _offerManager;
        public AppMenu(Repository<MenuItem> productRepo, Repository<Order> orderRepo, Repository<Offer> offerRepo, OfferManager offerManager)
        {
            _productRepo = productRepo;
            _orderRepo = orderRepo;
            _offerRepo = offerRepo;
            _offerManager = offerManager;
        }

        public void Run()
        {
            bool isRunning = true;
            int selectedIndex = 0;

            string[] options = {
                "Złóż nowe zamówienie",
                "Pokaż aktywne zamówienia",
                "Zobacz Menu",
                "Pokaż oferty",
                "Wyjdź"
            };


            while (isRunning)
            {
                Console.Clear();
                Console.WriteLine("=== SYSTEM OBSŁUGI ZAMÓWIEŃ ===");

                for (int i = 0; i < options.Length; i++) {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();

                    }
                    else {
                        Console.WriteLine($" {options[i]}");
                    
                    
                    }
                
                
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);



                switch (keyInfo.Key) {
                    case ConsoleKey.UpArrow:selectedIndex--;
                        if (selectedIndex < 0) selectedIndex = options.Length - 1;
                        break;

                    case ConsoleKey.DownArrow:selectedIndex++;
                        if (selectedIndex >= options.Length) selectedIndex = 0;
                        break;

                    case ConsoleKey.Enter:
                        switch (selectedIndex) {
                            case 0:
                                CreateNewOrder();
                                break;
                            case 1:
                                ShowOrders();
                                break;
                            case 2:
                                ShowProducts();
                                break;
                            case 3:
                                ShowOffers();
                                break;
                            case 4:
                                isRunning = false;
                                break;
                        }
                        break;        
                
                }
        
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
                Console.ReadKey(true);
                return;
            }

            Order newOrder = new Order();
            bool isOrdering = true;
            string lastAdded = "";
            int selectedIndex = 0;

            bool showingCategories = true;
            MenuItem.FoodCategory currentCategory = MenuItem.FoodCategory.None;
            List<MenuItem> currentItems = new List<MenuItem>();

            var categories = new List<MenuItem.FoodCategory> {
                MenuItem.FoodCategory.Starter,
                MenuItem.FoodCategory.MainCourse,
                MenuItem.FoodCategory.Dessert,
                MenuItem.FoodCategory.Drink
            };

            while (isOrdering)
            {
                Console.Clear();
                Console.WriteLine("=== TWORZENIE ZAMÓWIENIA ===");
                Console.WriteLine($"Aktualna kwota: {newOrder.CalculateFinalPrice():f2} zł");
                Console.WriteLine($"W koszyku: {newOrder.Positions.Sum(p => (int)p.Quantity)} szt.");

                if (!string.IsNullOrEmpty(lastAdded))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"(Ostatnio dodano: {lastAdded})");
                    Console.ResetColor();
                }
                Console.WriteLine();

                int totalOptions = showingCategories ? categories.Count + 1 : currentItems.Count + 1;

                for (int i = 0; i < totalOptions; i++)
                {
                    bool isSelected = (i == selectedIndex);

                    if (isSelected)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    if (showingCategories)
                    {
                        if (i == 0) Console.WriteLine(isSelected ? "> [ ZAKOŃCZ DOBIERANIE ]" : "  [ ZAKOŃCZ DOBIERANIE ]");
                        else Console.WriteLine(isSelected ? $"> {categories[i - 1]}" : $"  {categories[i - 1]}");
                    }
                    else
                    {
                        if (i == 0) Console.WriteLine(isSelected ? "> [ WRÓĆ ]" : "  [ WRÓĆ ]");
                        else
                        {
                            var item = currentItems[i - 1];
                            Console.WriteLine(isSelected ? $"> {item.Name} - {item.Price:f2} zł" : $"  {item.Name} - {item.Price:f2} zł");
                        }
                    }

                    if (isSelected) Console.ResetColor();
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex--;
                        if (selectedIndex < 0) selectedIndex = totalOptions - 1;
                        break;

                    case ConsoleKey.DownArrow:
                        selectedIndex++;
                        if (selectedIndex >= totalOptions) selectedIndex = 0;
                        break;

                    case ConsoleKey.Enter:
                        if (showingCategories)
                        {
                            if (selectedIndex == 0)
                            {
                                isOrdering = false;
                            }
                            else
                            {
                                currentCategory = categories[selectedIndex - 1];
                                currentItems = menuItems.Where(i => i.Type == currentCategory).ToList();
                                showingCategories = false;
                                selectedIndex = 0;
                            }
                        }
                        else
                        {
                            if (selectedIndex == 0)
                            {
                                showingCategories = true;
                                selectedIndex = 0;
                            }
                            else
                            {
                                var product = currentItems[selectedIndex - 1];
                                newOrder += product;
                                lastAdded = product.Name;
                            }
                        }
                        break;
                }
            }

            if (newOrder.Positions.Count > 0)
            {
                Console.Clear();
                Console.WriteLine("=== PODSUMOWANIE ZAMÓWIENIA ===");

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

            Console.ReadKey(true);
        }

        private void ShowOrders()
        {
            bool isViewing = true;
            int selectedIndex = 0;
            Order.OrderState currState = Order.OrderState.none;
            var allOrders = _orderRepo.GetAll();

            List<object> options = new() { "[ WRÓĆ DO MENU GŁÓWNEGO ]", "[ WYCZYŚĆ LISTĘ ZAMÓWIEŃ ]", "[inProgress] [finished] [canceled] [none]" };

            while (isViewing)
            {
                Console.Clear();
                Console.WriteLine("=== LISTA ZAMÓWIEŃ ===");

                if (allOrders.Count == 0)
                {
                    Console.WriteLine("\nBrak jakichkolwiek zamówień.");
                    Console.WriteLine("\nWciśnij enter aby wrócić....");
                    Console.ReadKey(true);
                    return;
                }

                var filteredOrders = currState == Order.OrderState.none ? allOrders : allOrders.Where(elem => elem.State == currState);
                List<Object> menuOptions = options.Concat(filteredOrders).ToList();

                if (selectedIndex >= menuOptions.Count) selectedIndex = menuOptions.Count - 1;

                for (int i = 0; i < menuOptions.Count; i++)
                {
                    object optionString = menuOptions[i];

                    if (menuOptions[i] is Order) 
                    {
                        Order order = (Order)menuOptions[i];
                        var price = _offerManager.CalculateFinalPriceWithDiscounts(order.Positions);

                        int itemsCount = order.Positions.Sum(p => (int)p.Quantity);

                        string commentInfo = string.IsNullOrEmpty(order.Comment) ? "Brak" : order.Comment;
                        if (commentInfo.Length > 10)
                        {
                            commentInfo = commentInfo.Substring(0, 10) + "...";
                        }

                        optionString = $"[{order.State}] Zamówienie ({itemsCount} poz.) | Do zapłaty: {price.finalPrice:f2} zł | Komentarz: {commentInfo}";
                    }

                    if (i == 2) 
                    {
                        var states = ((string)options[i]).Split(' ');
                    
                        for (int j = 0; j < 4; j++) 
                        {
                            if (j == (int)currState) 
                            {
                                ColoredConsole.Write(states[j], ConsoleColor.Green, ConsoleColor.Black);
                                continue;
                            }

                            Console.Write(states[j]);
                        }

                        Console.WriteLine();
                        continue;
                    }

                    if (i == selectedIndex)
                    {
                        ColoredConsole.WriteLine($"> {optionString}", ConsoleColor.Black, ConsoleColor.White);
                    }
                    else
                    {
                        Console.WriteLine($"  {optionString}");
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex--;
                        if (selectedIndex < 0) selectedIndex = menuOptions.Count - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex++;
                        if (selectedIndex >= menuOptions.Count) selectedIndex = 0;
                        break;
                    case ConsoleKey.Enter:
                        if (selectedIndex == 0)
                        {
                            isViewing = false;
                        }
                        else if (selectedIndex == 1)
                        {
                            Console.Clear();
                            Console.WriteLine("=== UWAGA ===");
                            Console.WriteLine("Czy na pewno chcesz usunąć WSZYSTKIE zamówienia z historii? (T/N):");
                            string confirm = Console.ReadLine()?.Trim().ToUpper() ?? "";

                            if (confirm == "T" || confirm == "TAK")
                            {
                                _orderRepo.GetAll().Clear();
                                _orderRepo.Save();
                            }
                        }
                        else if (selectedIndex == 2) 
                        {
                            var states = ((string)options[selectedIndex]).Split(' ');
                            currState = (Order.OrderState)(((int)currState + 1) % 4);
                        }
                        else
                        {
                            var selectedOrder = allOrders[selectedIndex - 2];
                            ManageOrder(selectedOrder);
                        }
                        break;
                }
            }
        }
        private void ManageOrder(Order order)
        {
            bool isManaging = true;
            int selectedIndex = 0;
            string[] options = { "Oznacz jako ZAKOŃCZONE", "Oznacz jako ANULOWANE", "Wróć" };

            while (isManaging)
            {
                Console.Clear();
                Console.WriteLine("=== SZCZEGÓŁY ZAMÓWIENIA ===\n");
                Console.WriteLine(order);
                var price = _offerManager.CalculateFinalPriceWithDiscounts(order.Positions);
                Console.WriteLine($"Cena po rabatach: {price.finalPrice:f2} zł (Naliczony rabat: {price.discount:f2} zł)\n");
                Console.WriteLine("Wybierz akcję dla tego zamówienia:");

                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine($"> {options[i]}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.WriteLine($"  {options[i]}");
                    }
                }

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.UpArrow:
                        selectedIndex--;
                        if (selectedIndex < 0) selectedIndex = options.Length - 1;
                        break;
                    case ConsoleKey.DownArrow:
                        selectedIndex++;
                        if (selectedIndex >= options.Length) selectedIndex = 0;
                        break;
                    case ConsoleKey.Enter:
                        if (selectedIndex == 0)
                        {
                            order.ChangeState(Order.OrderState.finished);
                            _orderRepo.Save();
                            isManaging = false;
                        }
                        else if (selectedIndex == 1)
                        {
                            order.ChangeState(Order.OrderState.canceled);
                            _orderRepo.Save();
                            isManaging = false;
                        }
                        else if (selectedIndex == 2)
                        {
                            isManaging = false;
                        }
                        break;
                }
            }
        }

        private void ShowProducts()
        {
            Console.Clear();
            Console.WriteLine("=== NASZE MENU ===");
            var items = _productRepo.GetAll();

            var sortedItems = items.OrderBy(i => i.Type).ToList();


            foreach (var item in sortedItems)
            {
                Console.WriteLine($"Nazwa: {item.Name,-25} | Cena: {item.Price:f2} zł | Kategoria: {item.Type}");
            }
            Console.WriteLine("\nWciśnij enter aby wrócić....");
            Console.ReadKey();
        }

        private void ShowOffers()
        {
            Console.Clear();
            Console.WriteLine("=== AKTUALNE OFERTY ===\n");
            var offers = _offerRepo.GetAll();

            if (offers.Count == 0)
            {
                Console.WriteLine("Brak dostępnych ofert.");
            }
            else
            {
                for (int i = 0; i < offers.Count; i++)
                {
                    Console.WriteLine($"Oferta #{i + 1}");
                    Console.WriteLine(offers[i]);
                    Console.WriteLine(new string('-', 40));
                }
            }

            Console.WriteLine("\nWciśnij enter aby wrócić....");
            Console.ReadKey(true);
        }
    }
}
