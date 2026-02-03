using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class Menu
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }

        public Menu(string menuId, string menuName)
        {
            MenuId = menuId;
            MenuName = menuName;
        }
        private List<FoodItem> foodItems = new List<FoodItem>();

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
            foreach (var item in foodItems)
            {
                System.Console.WriteLine(item);
            }
        }

        public override string ToString()
        {
            return $"Menu: {MenuName}";
        }
    }
}
