using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PRG_Final_ASG
{
    internal class Order
    {
        // canonical properties (from signature)
        public int OrderId { get; set; }
        public DateTime OrderDateTime { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime DeliverDateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderPaymentMethod { get; set; }
        public bool OrderPaid { get; set; }

        private List<OrderedFoodItem> orderedFoodItems = new List<OrderedFoodItem>();
        private List<SpecialOffer> specialOffers = new List<SpecialOffer>();

        // Full signature ctor (already in your signatures)
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

        // Parameterless ctor (Program uses this)
        public Order()
        {
            OrderDateTime = DateTime.MinValue;
            DeliverDateTime = DateTime.MinValue;
            OrderStatus = "Pending";
        }

        // Convenience ctor used in Program.LoadOrders (int, double, string)
                public Order(int orderId, double orderTotal, string orderStatus)
        {
            OrderId = orderId;
            OrderTotal = orderTotal;
            OrderStatus = orderStatus;
            OrderDateTime = DateTime.Now;
            DeliverDateTime = DateTime.Now;
        }

        // Compatibility aliases expected by Program.cs
        public double TotalAmount
        {
            get => OrderTotal;
            set => OrderTotal = value;
        }

        public DateTime DeliveryDateTime
        {
            get => DeliverDateTime;
            set => DeliverDateTime = value;
        }

        public string PaymentMethod
        {
            get => OrderPaymentMethod;
            set => OrderPaymentMethod = value;
        }

        // expose ordered items
        public List<OrderedFoodItem> OrderedItems => orderedFoodItems;

        // existing API
        public double CalculateOrderTotal()
        {
            double sum = 0;
            foreach (var it in orderedFoodItems)
            {
                sum += it.SubTotal;
            }
            OrderTotal = sum;
            return OrderTotal;
        }

        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            if (item == null) return;
            orderedFoodItems.Add(item);
            // update subtotal
            CalculateOrderTotal();
        }

        public override string ToString()
        {
            return $"Order {OrderId} - {OrderStatus} - ${OrderTotal:F2}";
        }
    }
}
