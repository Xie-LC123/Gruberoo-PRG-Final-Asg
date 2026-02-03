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

        public string ItemName => itemName;
        public double Price => price;

        public override string ToString()
        {
            return $"{itemName}: {description} - ${price:F2}";
        }
    }
}
