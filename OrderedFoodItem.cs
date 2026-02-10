using System;
using System.Collections.Generic;
using System.Text;

namespace PRG_Final_ASG
{
    internal class OrderedFoodItem : FoodItem
    {
        public int QtyOrdered { get; set; }

        // Computed property (always correct)
        public double SubTotal => QtyOrdered * Price;

        // Existing constructor
        public OrderedFoodItem(string itemName, string itemDesc, double itemPrice, string customise)
            : base(itemName, itemDesc, itemPrice, customise)
        {
            QtyOrdered = 1;
        }

        public OrderedFoodItem(string itemName, int qty)
            : base(itemName, "", 0, "")
        {
            QtyOrdered = qty;
        }

        public override string ToString()
        {
            return $"{ItemName} x{QtyOrdered} = ${SubTotal:F2}";
        }
    }

}

