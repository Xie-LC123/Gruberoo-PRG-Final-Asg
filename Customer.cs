using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class Customer
    {
        public string EmailAddress { get; set; }
        public string CustomerName { get; set; }

        public Customer(string emailAddress, string customerName)
        {
            EmailAddress = emailAddress;
            CustomerName = customerName;
        }

        private List<Order> orders = new List<Order>();

        public void AddOrder(Order order)
        {
            orders.Add(order);
        }

        public bool RemoveOrder(Order order)
        {
            return orders.Remove(order);
        }

        public void DisplayAllOrders()
        {
            foreach (var order in orders)
            {
                System.Console.WriteLine(order);
            }
        }

        public override string ToString()
        {
            return $"{CustomerName} ({EmailAddress})";
        }
    }
}
