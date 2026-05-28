using System;
using System.Collections.Generic;
using System.Text;

namespace Obiektuwa.Models {
    public class MenuItem {
        public enum FoodCategory {
            None,
            Starter,
            MainCourse,
            Dessert,
            Drink
        }
        public Guid ID { get; init; } = Guid.NewGuid();
        public string Name { get; init; }
        public double Price { get; init; }
        public FoodCategory Type { get; init; }

        public MenuItem(string name, double price, FoodCategory type = FoodCategory.None) {
            Name = name;
            Price = price;
            Type = type;
        }

        public override string ToString() {
            return $"ID: {ID.ToString()} Name: {Name} Price: {Price:f2}";
        }
    }
}
