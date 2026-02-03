using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class OrderedFoodItem : FoodItem
    {
        public OrderedFoodItem(string itemName, string itemDesc, double itemPrice, string customise) : base(itemName, itemDesc, itemPrice, customise)
        {
        }

        public int QtyOrdered { get; set; }
        public double SubTotal { get; private set; }

        public double CalculateSubtotal()
        {
            SubTotal = ItemPrice * QtyOrdered;
            return SubTotal;
        }

    }
}

