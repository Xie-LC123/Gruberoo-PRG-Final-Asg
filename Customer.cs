using System;
using System.Collections.Generic;
using System.Text;

namespace PRG_Final_ASG
{
    internal class Customer
    {
        // canonical fields / properties (existing signature)
        public string EmailAddress { get; set; }
        public string CustomerName { get; set; }

        private List<Order> orders = new List<Order>();

        // constructors
        public Customer(string emailAddress, string customerName)
        {
            EmailAddress = emailAddress;
            CustomerName = customerName;
        }

            // Compatibility aliases used by Program.cs
        public string Email
        {
            get => EmailAddress;
            set => EmailAddress = value;
        }

        public string Name
        {
            get => CustomerName;
            set => CustomerName = value;
        }

        // Expose orders list (read-only reference)
        public List<Order> OrderList => orders;

        // existing API
        public void AddOrder(Order order)
        {
            if (order == null) return;
            orders.Add(order);

            // keep a back-reference if needed by Program (some code expects Order.Customer)
            try
            {
                // set Order's customer if property exists (optional)
                var prop = order.GetType().GetProperty("Customer");
                if (prop != null && prop.PropertyType == typeof(Customer))
                {
                    prop.SetValue(order, this);
                }
            }
            catch { /* ignore reflection failures */ }
        }

        public bool RemoveOrder(Order order)
        {
            return orders.Remove(order);
        }

        public void DisplayAllOrders()
        {
            foreach (var o in orders)
            {
                Console.WriteLine(o.ToString());
            }
        }

        public override string ToString()
        {
            return $"{CustomerName} ({EmailAddress})";
        }
    }
}
