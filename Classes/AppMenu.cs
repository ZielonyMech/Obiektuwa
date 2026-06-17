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
                "Dodaj nową ofertę",
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
                                CreateNewOffer();
                                break;
                            case 5:
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
            var offersList = _offerRepo.GetAll();

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
            bool showingOffers = false;
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

                int totalOptions;
                if (showingCategories) totalOptions = categories.Count + 2;
                else if (showingOffers) totalOptions = offersList.Count + 1;
                else totalOptions = currentItems.Count + 1;

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
                        else if (i == categories.Count + 1) Console.WriteLine(isSelected ? "> Offers" : "  Offers");
                        else Console.WriteLine(isSelected ? $"> {categories[i - 1]}" : $"  {categories[i - 1]}");
                    }
                    else if (showingOffers)
                    {
                        if (i == 0) Console.WriteLine(isSelected ? "> [ WRÓĆ ]" : "  [ WRÓĆ ]");
                        else
                        {
                            string offerInfo = $"Oferta #{i}";
                            Console.WriteLine(isSelected ? $"> {offerInfo}" : $"  {offerInfo}");
                        }
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
                            else if (selectedIndex == categories.Count + 1)
                            {
                                showingCategories = false;
                                showingOffers = true;
                                selectedIndex = 0;
                            }
                            else
                            {
                                currentCategory = categories[selectedIndex - 1];
                                currentItems = menuItems.Where(i => i.Type == currentCategory).ToList();
                                showingCategories = false;
                                showingOffers = false;
                                selectedIndex = 0;
                            }
                        }
                        else if (showingOffers)
                        {
                            if (selectedIndex == 0)
                            {
                                showingCategories = true;
                                showingOffers = false;
                                selectedIndex = 0;
                            }
                            else
                            {
                                var offer = offersList[selectedIndex - 1];

                                foreach (var (item, quantity) in offer.RequiredPositions)
                                {
                                    for (int q = 0; q < quantity; q++)
                                    {
                                        newOrder += item;
                                    }
                                }

                                lastAdded = $"Oferta #{selectedIndex}";
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

                Console.WriteLine("\nCzy klient ma zniżkę studencką? (T/N):");
                string studentInput = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (studentInput == "T" || studentInput == "TAK")
                {
                    newOrder.HasStudentDiscount = true;
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
                        var price = _offerManager.CalculateFinalPriceWithDiscounts(order);

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

                        if (i == selectedIndex) Console.Write("> ");
                        else Console.Write(" ");

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
                        Console.WriteLine($" {optionString}");
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
                            var selectedOrder = (Order)menuOptions[selectedIndex];
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
                var price = _offerManager.CalculateFinalPriceWithDiscounts(order);
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
        private void CreateNewOffer()
        {
            var menuItems = _productRepo.GetAll();

            if (menuItems.Count == 0)
            {
                Console.Clear();
                Console.WriteLine("Menu jest puste, nie można stworzyć oferty :/");
                Console.WriteLine("Wciśnij enter....");
                Console.ReadKey(true);
                return;
            }
            List<(MenuItem Item, uint Quantity)> offerPositions = new();

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
                Console.WriteLine("=== TWORZENIE NOWEJ OFERTY ===");
                int totalItems = (int)offerPositions.Sum(p => p.Quantity);
                Console.WriteLine($"W ofercie: {totalItems} szt.");

                if (!string.IsNullOrEmpty(lastAdded))
                {
                    ColoredConsole.WriteLine($"(Ostatnio dodano: {lastAdded})", ConsoleColor.Green, ConsoleColor.Black);
                    Console.WriteLine();
                }

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
                            if (selectedIndex == 0) isOrdering = false;
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
                                var existingIndex = offerPositions.FindIndex(p => p.Item.ID == product.ID);
                                if (existingIndex >= 0)
                                {
                                    var existing = offerPositions[existingIndex];
                                    offerPositions[existingIndex] = (existing.Item, existing.Quantity + 1);
                                }
                                else
                                {
                                    offerPositions.Add((product, 1));
                                }
                                lastAdded = product.Name;
                            }
                        }
                        break;
                }
            }
            if (offerPositions.Count > 0)
            {
                Console.Clear();
                Console.WriteLine("=== USTAWIANIE ZNIŻKI DLA OFERTY ===");
                Offer.DiscountType selectedDiscountType = Offer.DiscountType.Percent;
                Console.WriteLine("Wybierz rodzaj zniżki:");
                Console.WriteLine("1. Stała cena za cały zestaw (FixedPrice)");
                Console.WriteLine("2. Zniżka procentowa na zestaw (Percent)");
                Console.WriteLine("3. Zniżka kwotowa od całości (FixedAmount)");

                while (true)
                {
                    Console.Write("\nTwój wybór (1-3): ");
                    string typeInput = Console.ReadLine()?.Trim() ?? "";
                    if (typeInput == "1") { selectedDiscountType = Offer.DiscountType.FixedPrice; break; }
                    if (typeInput == "2") { selectedDiscountType = Offer.DiscountType.Percent; break; }
                    if (typeInput == "3") { selectedDiscountType = Offer.DiscountType.FixedAmount; break; }
                    Console.WriteLine("Nieprawidłowy wybór. Wpisz 1, 2 lub 3.");
                }

                double discountValue = 0;
                while (true)
                {
                    Console.Write("\nPodaj wartość zniżki (np. 15 dla 15% lub 19,99 dla ceny): ");
                    string valInput = Console.ReadLine()?.Trim() ?? "";
                    if (double.TryParse(valInput, out discountValue) && discountValue >= 0)
                    {
                        break;
                    }
                    Console.WriteLine("Nieprawidłowa wartość.");
                }

                Console.WriteLine("\nZapisywanie oferty....");
                Offer newOffer = new Offer(offerPositions, (selectedDiscountType, discountValue));
                _offerRepo.Add(newOffer);
                _offerRepo.Save();
                Console.WriteLine("Oferta dodana pomyślnie! Wciśnij enter aby zakończyć....");
            }
            else
            {
                Console.WriteLine("\nAnulowano tworzenie pustej oferty. Wciśnij enter aby zakończyć...");
            }
            Console.ReadKey(true);
        }
    }
}
