using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRG_Final_ASG;

namespace Gruberoo
{
    internal class OrderedFoodItem : FoodItem
    {
        public OrderedFoodItem(string itemName, string itemDesc, double itemPrice, string customise) : base(itemName, itemDesc, itemPrice, customise)
        {
        }

        public int QtyOrdered { get; set; }
        public double SubTotal { get; private set; }
        public int ItemPrice { get; private set; }

        public double CalculateSubtotal()
        {
            SubTotal = ItemPrice * QtyOrdered;
            return SubTotal;
        }

    }
}

