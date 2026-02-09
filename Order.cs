using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PRG_Final_ASG;

namespace Gruberoo
{
    internal class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime DeliverDateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderPaymentMethod { get; set; }
        public bool OrderPaid { get; set; }
        public DateTime DeliveryDateTime { get; internal set; }
        public double TotalAmount { get; internal set; }

        public Order(int orderId, DateTime orderDateTime, double orderTotal, string orderStatus, DateTime deliverDateTime, string deliveryAddress, string orderPaymentMethod, bool orderPaid)
        {
            OrderId = orderId;
            OrderDateTime = orderDateTime;
            OrderTotal = orderTotal;
            OrderStatus = orderStatus;
            DeliverDateTime = deliverDateTime;
            DeliveryAddress = deliveryAddress;
            OrderPaymentMethod = orderPaymentMethod;
            OrderPaid = orderPaid;
        }
        private List<OrderedFoodItem> orderedFoodItems = new List<OrderedFoodItem>();
        private List<SpecialOffer> specialOffers = new List<SpecialOffer>();

        public double CalculateOrderTotal()
        {
            double total = 0;
            foreach (var item in orderedFoodItems)
            {
                total += item.CalculateSubtotal();
            }

            foreach (var offer in specialOffers)
            {
                total -= offer.Discount;
            }

            OrderTotal = total;
            return OrderTotal;
        }
        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            orderedFoodItems.Add(item);
        }   

    }
}
