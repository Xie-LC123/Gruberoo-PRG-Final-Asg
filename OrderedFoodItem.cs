using System;
using System.Collections.Generic;
using System.Text;

namespace PRG_Final_ASG
{
    internal class OrderedFoodItem : FoodItem
    {
        public int QtyOrdered { get; set; }
        public double SubTotal { get; set; }

        // Existing signature ctor (keep)
        public OrderedFoodItem(string itemName, string itemDesc, double itemPrice, string customise) : base(itemName, itemDesc, itemPrice, customise)
        {
            QtyOrdered = 1;
            SubTotal = CalculateSubtotal();
        }

        // Convenience ctor so Program (or other code) can create from a FoodItem + qty
        public OrderedFoodItem(string itemName, int qty) : base(itemName, "", 0, "")
        {
            QtyOrdered = qty;
            SubTotal = CalculateSubtotal();
        }
        public void UpdateSubTotal()
        {
            SubTotal = QtyOrdered * Price;
        }


        public double CalculateSubtotal()
        {
            return QtyOrdered * (base.Price);
        }

        public override string ToString()
        {
            return $"{ItemName} x{QtyOrdered} = ${SubTotal:F2}";
        }
    }
}

