using System;
using System.Collections.Generic;
using System.Text;

namespace PRG_Final_ASG
{
    internal class FoodItem
    {
        private string itemName;
        private string description;
        private double price;

        public FoodItem(string itemName, string description, double price)
        {
            this.itemName = itemName;
            this.description = description;
            this.price = price;
        }



        // Existing ctor with extra 'customise' parameter (kept for compatibility)
        public FoodItem(string itemName, string description, double price, string customise)
        {
            this.itemName = itemName;
            this.description = description;
            this.price = price;
        }

        public string ItemName => itemName;
        public double Price => price;

        // expose description if needed (OrderedFoodItem may use it)
        public string Description => description;

        public override string ToString()
        {
            return $"{itemName}: {description} - ${price:F2}";
        }
    }
}
