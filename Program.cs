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
using System.Text;
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

            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

            Console.WriteLine($"{restaurants.Count} restaurants loaded!");

            int totalFoodItems = restaurants.Sum(r => r.FoodItems.Count);
            Console.WriteLine($"{totalFoodItems} food items loaded!");

            Console.WriteLine($"{customers.Count} customers loaded!");

            int totalOrders = customers.Sum(c => c.OrderList.Count);
            Console.WriteLine($"{totalOrders} orders loaded!");

            Console.WriteLine();
            Console.ReadKey();
            Console.Clear();

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
                    case "8": LoadFoodItems(); break;
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
            string filePath = "customers.csv";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("customers.csv NOT FOUND");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            int loaded = 0;

            for (int i = 1; i < lines.Length; i++) // skip header
            {
                var data = lines[i].Split(',');

                if (data.Length != 2) continue; // skip lines with unexpected format

                try
                {
                    string name = data[0].Trim();
                    string email = data[1].Trim();

                    // Add customer
                    customers.Add(new Customer(email, name));
                    loaded++;
                }
                catch
                {
                    // silently skip invalid lines
                    continue;
                }
            }

            Console.WriteLine($"Customers loaded: {loaded}");
        }



        static void LoadRestaurants()
        {
            if (!File.Exists("restaurants.csv"))
            {
                Console.WriteLine("restaurants.csv not found");
                return;
            }

            var lines = File.ReadAllLines("restaurants.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                var data = lines[i].Split(',');

                // ✅ 3 arguments
                restaurants.Add(
                    new Restaurant(
                        data[0].Trim(), // RestaurantId
                        data[1].Trim(), // RestaurantName
                        data[2].Trim()  // RestaurantEmail
                    )
                );
            }

            Console.WriteLine($"Restaurants loaded: {restaurants.Count}");
        }

        static void LoadFoodItems()
        {
            if (!File.Exists("fooditems.csv"))
            {
                Console.WriteLine("fooditems.csv not found");
                return;
            }

            var lines = File.ReadAllLines("fooditems.csv");

            for (int i = 1; i < lines.Length; i++) // skip header
            {
                var data = lines[i].Split(',');


                string restaurantId = data[0].Trim();
                string itemName = data[1].Trim();
                string description = data[2].Trim();
                string priceStr = data[3].Trim();

                double.TryParse(priceStr, out double price);


                Restaurant r = restaurants.Find(x => x.RestaurantId.Trim() == restaurantId);
                if (r == null) continue;

                r.AddFoodItem(new FoodItem(itemName, description, price));
            }
        }
        static void LoadOrders()
        {
            string filePath = "orders.csv";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("orders.csv NOT FOUND");
                return;
            }

            var lines = File.ReadAllLines(filePath);
            int loaded = 0;

            for (int i = 1; i < lines.Length; i++) // skip header
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // Custom split to handle quoted fields (especially "Items")
                List<string> data = new List<string>();
                bool inQuotes = false;
                StringBuilder sb = new StringBuilder();

                foreach (char c in line)
                {
                    if (c == '"')
                    {
                        inQuotes = !inQuotes;
                        continue;
                    }

                    if (c == ',' && !inQuotes)
                    {
                        data.Add(sb.ToString());
                        sb.Clear();
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                data.Add(sb.ToString()); // last element

                if (data.Count < 10) continue; // skip incomplete lines

                try
                {
                    int orderId = int.Parse(data[0].Trim());
                    string customerEmail = data[1].Trim();
                    string restaurantId = data[2].Trim();

                    DateTime deliveryDateTime = DateTime.ParseExact(
                        $"{data[3].Trim()} {data[4].Trim()}",
                        "dd/MM/yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture
                    );

                    DateTime createdDateTime = DateTime.ParseExact(
                        data[6].Trim(),
                        "dd/MM/yyyy HH:mm",
                        System.Globalization.CultureInfo.InvariantCulture
                    );

                    double totalAmount = double.Parse(data[7].Trim());
                    string status = data[8].Trim();
                    string items = data[9].Trim(); // all remaining items combined

                    // Create Order object
                    Order order = new Order
                    {
                        OrderId = orderId,
                        OrderDateTime = createdDateTime,
                        DeliveryDateTime = deliveryDateTime,
                        TotalAmount = totalAmount,
                        OrderStatus = status
                    };

                    // Handle Customer
                    Customer c = customers.Find(x => x.Email == customerEmail);
                    if (c == null)
                    {
                        string generatedName = customerEmail.Split('@')[0]
                                                        .Replace('.', ' ')
                                                        .Replace('_', ' ');
                        generatedName = string.Join(" ", generatedName
                            .Split(' ')
                            .Select(word => char.ToUpper(word[0]) + word.Substring(1)));

                        c = new Customer(generatedName, customerEmail);
                        customers.Add(c);
                    }

                    c.AddOrder(order);

                    // Handle Restaurant
                    Restaurant r = restaurants.Find(x => x.RestaurantId == restaurantId);
                    if (r != null)
                    {
                        r.AddOrderToQueue(order);
                        order.Restaurant = r;
                    }

                    loaded++;
                }
                catch
                {
                    // silently skip invalid lines
                    continue;
                }
            }

            Console.WriteLine($"Orders loaded: {loaded}");
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
            Console.WriteLine("All Restaurants and Menu Items");
            Console.WriteLine("==============================");

            foreach (Restaurant r in restaurants)
            {
                Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");

                if (r.FoodItems.Count == 0)
                {
                    Console.WriteLine("- No menu items available");
                }
                else
                {
                    foreach (FoodItem f in r.FoodItems)
                    {
                        Console.WriteLine(
                            $"- {f.ItemName}: {f.Description} - ${f.Price:F2}"
                        );
                    }
                }

                Console.WriteLine(); // spacing between restaurants
            }
        }


        // =========================
        // FEATURE 4
        // =========================
        static void ListAllOrders()
{
    Console.WriteLine("All Orders");
    Console.WriteLine("==========");
    Console.WriteLine("{0,-8} {1,-15} {2,-15} {3,-18} {4,-8} {5,-10}",
        "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
    Console.WriteLine("{0,-8} {1,-15} {2,-15} {3,-18} {4,-8} {5,-10}",
        "--------", "---------------", "---------------", "------------------", "------", "---------");

    // Flatten all orders from all customers
    var allOrders = customers
        .SelectMany(c => c.OrderList.Select(o => new { Order = o, CustomerName = c.Name }))
        .OrderBy(x => x.Order.OrderId); // sort globally by OrderId

    foreach (var x in allOrders)
    {
        string restaurantName = x.Order.Restaurant?.RestaurantName ?? "Unknown";

        Console.WriteLine("{0,-8} {1,-15} {2,-15} {3,-18} {4,-8:C} {5,-10}",
            x.Order.OrderId,
            x.CustomerName,
            restaurantName,
            x.Order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"),
            x.Order.TotalAmount,
            x.Order.OrderStatus);
    }

    Console.WriteLine();
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