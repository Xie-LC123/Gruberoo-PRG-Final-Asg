//==========================================================
// Student Number : S10271327
// Student Name : Xie Liangchen
// Student Number : S10273654
// Student Name : Chiam Sheng Le
//==========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PRG_Final_ASG;

namespace Gruberoo
{
    class Program
    {
        static List<Customer> customers = new List<Customer>();
        static List<Restaurant> restaurants = new List<Restaurant>();
        static Stack<Order> refundStack = new Stack<Order>();
        static int nextOrderId = 1000;

        static void Main(string[] args)
        {
            LoadCustomers();
            LoadRestaurants();
            LoadFoodItems();
            LoadOrders();

            string choice;

            do
            {
                Console.Clear();
                Console.WriteLine("===== Gruberoo Food Delivery System =====");
                Console.WriteLine("1. List all restaurants and menu items");
                Console.WriteLine("2. List all orders");
                Console.WriteLine("3. Create a new order");
                Console.WriteLine("4. Process an order");
                Console.WriteLine("5. Modify an existing order");
                Console.WriteLine("6. Delete an existing order");
                Console.WriteLine("7. Bulk Process Orders (Advanced A)");
                Console.WriteLine("0. Exit");
                Console.Write("Enter choice: ");
                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": ListAllRestaurants(); break;
                    case "2": ListAllOrders(); break;
                    case "3": CreateOrder(); break;
                    case "4": ProcessOrder(); break;
                    case "5": ModifyOrder(); break;
                    case "6": DeleteOrder(); break;
                    case "7": BulkProcessOrders(); break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();

            } while (choice != "0");
        }

        // =========================
        // FEATURE 1 & 2 – LOAD FILES
        // =========================

        static void LoadCustomers()
        {
            if (!File.Exists("customers.csv")) return;
            var lines = File.ReadAllLines("customers.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                var data = lines[i].Split(',');
                customers.Add(new Customer(data[0], data[1]));
            }
        }

        static void LoadRestaurants()
        {
            if (!File.Exists("restaurants.csv")) return;
            var lines = File.ReadAllLines("restaurants.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                var data = lines[i].Split(',');
                restaurants.Add(new Restaurant(data[0], data[1], data[2]));
            }
        }

        static void LoadFoodItems()
        {
            if (!File.Exists("fooditems.csv")) return;
            var lines = File.ReadAllLines("fooditems.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                var data = lines[i].Split(',');
                FoodItem item = new FoodItem(data[0], data[1], double.Parse(data[2]));

                Restaurant r = restaurants.Find(x => x.RestaurantId == data[3]);
                if (r != null) r.AddFoodItem(item);
            }
        }

        static void LoadOrders()
        {
            if (!File.Exists("orders.csv")) return;
            var lines = File.ReadAllLines("orders.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                var data = lines[i].Split(',');

                Order order = new Order(
                    int.Parse(data[0]),
                    double.Parse(data[4]),
                    data[5]
                );

                Customer c = customers.Find(x => x.Email == data[1]);
                Restaurant r = restaurants.Find(x => x.RestaurantId == data[2]);

                if (c != null) c.AddOrder(order);
                if (r != null) r.AddOrderToQueue(order);
            }
        }

        static int GenerateNewOrderId()
        {
            return ++nextOrderId;
        }

        // =========================
        // FEATURE 3
        // =========================
        static void ListAllRestaurants()
        {
            foreach (Restaurant r in restaurants)
            {
                Console.WriteLine($"\n{r.RestaurantName} ({r.RestaurantId})");

                foreach (FoodItem f in r.FoodItems)
                {
                    Console.WriteLine($"- {f.ItemName} - ${f.Price:F2}");
                }
            }
        }

        // =========================
        // FEATURE 4
        // =========================
        static void ListAllOrders()
        {
            foreach (Customer c in customers)
            {
                foreach (Order o in c.OrderList)
                {
                    Console.WriteLine(
                        $"{o.OrderId} | {c.Name} | ${o.TotalAmount:F2} | {o.OrderStatus}"
                    );
                }
            }
        }

        // =========================
        // FEATURE 5
        // =========================
        static void CreateOrder()
        {
            Console.Write("Customer Email: ");
            string email = Console.ReadLine();

            Customer cust = customers.Find(x => x.Email == email);
            if (cust == null) return;

            Console.Write("Restaurant ID: ");
            string restId = Console.ReadLine();

            Restaurant rest = restaurants.Find(x => x.RestaurantId == restId);
            if (rest == null) return;

            Order order = new Order();
            order.OrderId = GenerateNewOrderId();
            order.OrderStatus = "Pending";
            order.OrderDateTime = DateTime.Now;

            double total = 0;

            foreach (FoodItem f in rest.FoodItems)
            {
                Console.WriteLine($"{f.ItemName} - ${f.Price}");
                total += f.Price; // simple version
            }

            order.TotalAmount = total + 5;

            cust.AddOrder(order);
            rest.AddOrderToQueue(order);

            Console.WriteLine($"Order {order.OrderId} created!");
        }

        // =========================
        // FEATURE 6
        // =========================
        static void ProcessOrder()
        {
            Console.Write("Restaurant ID: ");
            string id = Console.ReadLine();

            Restaurant r = restaurants.Find(x => x.RestaurantId == id);
            if (r == null) return;

            foreach (Order o in r.OrderQueue)
            {
                Console.WriteLine($"{o.OrderId} - {o.OrderStatus}");
                Console.Write("[C]onfirm / [R]eject / [D]eliver: ");
                string choice = Console.ReadLine().ToUpper();

                if (choice == "C" && o.OrderStatus == "Pending")
                    o.OrderStatus = "Preparing";

                else if (choice == "R" && o.OrderStatus == "Pending")
                {
                    o.OrderStatus = "Rejected";
                    refundStack.Push(o);
                }

                else if (choice == "D" && o.OrderStatus == "Preparing")
                    o.OrderStatus = "Delivered";
            }
        }

        // =========================
        // FEATURE 7
        // =========================
        static void ModifyOrder()
        {
            Console.Write("Customer Email: ");
            string email = Console.ReadLine();

            Customer cust = customers.Find(x => x.Email == email);
            if (cust == null) return;

            var pending = cust.OrderList.Where(o => o.OrderStatus == "Pending").ToList();
            foreach (var o in pending)
                Console.WriteLine(o.OrderId);

            Console.Write("Order ID: ");
            int id = int.Parse(Console.ReadLine());

            Order order = pending.Find(o => o.OrderId == id);
            if (order == null) return;

            Console.Write("New Total Amount: ");
            order.TotalAmount = double.Parse(Console.ReadLine());

            Console.WriteLine("Order updated.");
        }

        // =========================
        // FEATURE 8
        // =========================
        static void DeleteOrder()
        {
            Console.Write("Customer Email: ");
            string email = Console.ReadLine();

            Customer cust = customers.Find(x => x.Email == email);
            if (cust == null) return;

            var pending = cust.OrderList.Where(o => o.OrderStatus == "Pending").ToList();

            foreach (var o in pending)
                Console.WriteLine(o.OrderId);

            Console.Write("Order ID: ");
            int id = int.Parse(Console.ReadLine());

            Order order = pending.Find(o => o.OrderId == id);
            if (order == null) return;

            order.OrderStatus = "Cancelled";
            refundStack.Push(order);

            Console.WriteLine("Order cancelled.");
        }

        // =========================
        // ADVANCED FEATURE A
        // =========================
        static void BulkProcessOrders()
        {
            DateTime now = DateTime.Now;
            int processed = 0;
            int preparing = 0;
            int rejected = 0;

            foreach (Restaurant r in restaurants)
            {
                foreach (Order o in r.OrderQueue)
                {
                    if (o.OrderStatus == "Pending")
                    {
                        processed++;
                        if ((o.DeliveryDateTime - now).TotalMinutes < 60)
                        {
                            o.OrderStatus = "Rejected";
                            rejected++;
                        }
                        else
                        {
                            o.OrderStatus = "Preparing";
                            preparing++;
                        }
                    }
                }
            }

            Console.WriteLine($"Processed: {processed}");
            Console.WriteLine($"Preparing: {preparing}");
            Console.WriteLine($"Rejected: {rejected}");
        }
    }
}