using System;
using System.Collections.Generic;
using System.Text;

namespace PRG_Final_ASG
{
    internal class Menu
    {
        private string menuId;
        private string menuName;
        private List<FoodItem> foodItems;

        public Menu(string menuId, string menuName)
        {
            this.menuId = menuId;
            this.menuName = menuName;
            foodItems = new List<FoodItem>();
        }

        public void AddFoodItem(FoodItem foodItem)
        {
            foodItems.Add(foodItem);
        }

        public bool RemoveFoodItem(FoodItem foodItem)
        {
            return foodItems.Remove(foodItem);
        }

        public void DisplayFoodItems()
        {
            foreach (FoodItem item in foodItems)
            {
                Console.WriteLine(item);
            }
        }

        public override string ToString()
        {
            return $"{menuName} ({menuId})";
        }

        public List<FoodItem> GetFoodItems()
        {
            return foodItems;
        }
    }
}
