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
        private string v1;
        private string v2;
        private double v3;

        public FoodItem(string v1, string v2, double v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
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
