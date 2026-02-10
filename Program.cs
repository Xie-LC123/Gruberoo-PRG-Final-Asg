//==========================================================
// Student Number : S10273654
// Student Name : Chiam Sheng Le
// Partner Name : Xie Liangchen
//==========================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

            LoadRestaurants();
            LoadFoodItems();
            LoadCustomers();
            LoadOrders();

            string choice;

            do
            {
                Console.WriteLine("\n===== Main Menu =====");
                Console.WriteLine("1. List all restaurants and menu items");
                Console.WriteLine("2. List all orders");
                Console.WriteLine("3. Create a new order");
                Console.WriteLine("4. Process an order");
                Console.WriteLine("5. Modify an existing order");
                Console.WriteLine("6. Delete an existing order");
                Console.WriteLine("7. Bulk Process Orders (Advanced A)");
                Console.WriteLine("8. Display total order amount (Advanced B)");
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
                    case "8": DisplayTotalOrderAmount(); break;

                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
                Console.Clear(); // clear after the first iteration

            } while (choice != "0");
            { 
            
            }
        }


        // =========================
        // FEATURE 1 & 2 – LOAD FILES (1 BY: Chiam Sheng Le)
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

            Console.WriteLine($"{loaded} customers loaded!");
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

            Console.WriteLine($"{restaurants.Count} restaurants loaded!");
        }

        static void LoadFoodItems()
        {
            if (!File.Exists("fooditems.csv"))
            {
                Console.WriteLine("fooditems.csv not found");
                return;
            }

            var lines = File.ReadAllLines("fooditems.csv");
            int loadedCount = 0;
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
                loadedCount++; // increment counter
            }
            Console.WriteLine($"{loadedCount} food items loaded!");
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

            // Regex to split CSV, handles quoted fields
            Regex csvSplit = new Regex(@",(?=(?:[^""]*""[^""]*"")*[^""]*$)");

            for (int i = 1; i < lines.Length; i++) // skip header
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                // Split line using regex
                string[] data = csvSplit.Split(line);

                // Remove surrounding quotes if present
                for (int j = 0; j < data.Length; j++)
                {
                    data[j] = data[j].Trim().Trim('"');
                }

                if (data.Length < 10) continue; // skip incomplete lines

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


                    string status = data[8].Trim();
                    string items = data[9].Trim(); // Items field as string

                    // Create Order object
                    Order order = new Order
                    {
                        OrderId = orderId,
                        OrderDateTime = createdDateTime,
                        DeliveryDateTime = deliveryDateTime,
                        OrderStatus = status
                    };

                    // Parse Items into OrderedItems list
                    if (!string.IsNullOrEmpty(items))
                    {
                        var itemList = items.Split('|'); // split by pipe

                        foreach (var item in itemList)
                        {
                            var parts = item.Split(','); // split name and quantity
                            if (parts.Length == 2)
                            {
                                string itemName = parts[0].Trim();
                                if (int.TryParse(parts[1].Trim(), out int quantity))
                                {
                                    // Find food item price from restaurant menu
                                    Restaurant rest = restaurants.Find(x => x.RestaurantId == restaurantId);
                                    FoodItem food = rest?.FoodItems.Find(f => f.ItemName == itemName);

                                    double price = food != null ? food.Price : 0;

                                    OrderedFoodItem orderedItem = new OrderedFoodItem(
                                        itemName,
                                        food?.Description ?? "",
                                        price,
                                        ""
                                    )
                                    {
                                        QtyOrdered = quantity
                                    };

                                    order.AddOrderedFoodItem(orderedItem);

                                }
                            }
                        }
                    }

                    order.TotalAmount = order.OrderedItems.Sum(w => w.SubTotal);
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

            Console.WriteLine($"{loaded} orders loaded!");
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
        // FEATURE 4 (BY: Chiam Sheng Le)
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
                    x.Order.TotalAmount+5,
                    x.Order.OrderStatus);
            }

            Console.WriteLine();

        }
        static void PrintOrders(IEnumerable<(Order order, string customerName)> orders, string title = "Orders List")
        {
            Console.WriteLine(title);
            Console.WriteLine("==========");
            Console.WriteLine("{0,-8} {1,-15} {2,-15} {3,-18} {4,-8} {5,-10}",
                "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
            Console.WriteLine("{0,-8} {1,-15} {2,-15} {3,-18} {4,-8} {5,-10}",
                "--------", "---------------", "---------------", "------------------", "------", "---------");

            foreach (var x in orders.OrderBy(x => x.order.OrderId))
            {
                string restaurantName = x.order.Restaurant?.RestaurantName ?? "Unknown";

                Console.WriteLine("{0,-8} {1,-15} {2,-15} {3,-18} {4,-8:C} {5,-10}",
                    x.order.OrderId,
                    x.customerName,
                    restaurantName,
                    x.order.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm"),
                    x.order.TotalAmount+5,
                    x.order.OrderStatus);
            }

            Console.WriteLine();
        }

        static void ListOrdersByRestaurant(Restaurant r)
        {
            if (r == null)
            {
                Console.WriteLine("Restaurant not found!");
                return;
            }

            var ordersForRestaurant = customers
                .SelectMany(c => c.OrderList
                    .Where(o => o.Restaurant == r)
                    .Select(o => (order: o, customerName: c.Name)));

            Console.WriteLine($"Orders for {r.RestaurantName}");
        }
        static void ListPendingOrders()
        {
            var pendingOrders = customers
                .SelectMany(c => c.OrderList
                    .Where(o => o.OrderStatus == "Pending")
                    .Select(o => (order: o, customerName: c.Name)));

            Console.WriteLine("Pending Orders");
            PrintOrders(pendingOrders); // reuse the same method
        }

        static void ListAllOrdersInfo()
        {
            var allOrders = customers
                .SelectMany(c => c.OrderList.Select(o => (order: o, customerName: c.Name)));

            PrintOrders(allOrders); // reuse the same method
        }
        // =========================
        // FEATURE 5
        // =========================
        static int GenerateNewOrderId()
        {
            return ++nextOrderId;
        }
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
        // FEATURE 6 (BY: Chiam Sheng Le)
        // =========================
        static void ProcessOrder()
        {
            Console.Write("Enter Restaurant ID: ");
            string restaurantId = Console.ReadLine().Trim();

            // Find restaurant
            Restaurant r = restaurants.Find(x => x.RestaurantId == restaurantId);
            if (r == null)
            {
                Console.WriteLine("Restaurant not found!");
                return;
            }

            // Get all orders for this restaurant with customer info, exclude Delivered, sorted by OrderId
            var ordersToProcess = customers
                .SelectMany(c => c.OrderList
                    .Where(o => o.Restaurant == r && o.OrderStatus != "Delivered")
                    .Select(o => (order: o, customerName: c.Name)))
                .OrderBy(x => x.order.OrderId)
                .ToList();

            if (!ordersToProcess.Any())
            {
                Console.WriteLine("No pending orders found for this restaurant.");
                return;
            }

            Console.WriteLine($"\nOrders for {r.RestaurantName}:");
            Console.WriteLine("===================================");

            foreach (var x in ordersToProcess)
            {
                Order o = x.order;

                while (true) // Keep prompting until a valid action occurs
                {
                    Console.WriteLine($"Processing Order {o.OrderId}:");
                    Console.WriteLine($"Customer: {x.customerName}");
                    Console.WriteLine("Ordered Items:");

                    if (o.OrderedItems.Count > 0)
                    {
                        int count = 1;
                        foreach (var item in o.OrderedItems)
                        {
                            Console.WriteLine($"{count}. {item.ItemName} - {item.QtyOrdered}");
                            count++;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No items found.");
                    }

                    Console.WriteLine($"Delivery date/time: {o.DeliveryDateTime:dd/MM/yyyy HH:mm}");
                    Console.WriteLine($"Total Amount: ${o.TotalAmount:F2}");
                    Console.WriteLine($"Order Status: {o.OrderStatus}");

                    // Show only valid actions
                    List<string> options = new List<string>();
                    if (o.OrderStatus == "Pending") options.Add("[C]onfirm");
                    if (o.OrderStatus == "Pending") options.Add("[R]eject");
                    if (o.OrderStatus == "Cancelled") options.Add("[S]kip");
                    if (o.OrderStatus == "Preparing") options.Add("[D]eliver");

                    Console.Write($"{string.Join("/", options)}: ");
                    string choice = Console.ReadLine().ToUpper();

                    bool validAction = false;

                    switch (choice)
                    {
                        case "C":
                            if (o.OrderStatus == "Pending")
                            {
                                o.OrderStatus = "Preparing";
                                Console.WriteLine($"Order {o.OrderId} confirmed. Status: {o.OrderStatus}");
                                validAction = true;
                            }
                            else Console.WriteLine("Cannot confirm: Order is not Pending.");
                            break;

                        case "R":
                            if (o.OrderStatus == "Pending")
                            {
                                o.OrderStatus = "Rejected";
                                refundStack.Push(o);
                                Console.WriteLine($"Order {o.OrderId} rejected. Status: {o.OrderStatus}");
                                validAction = true;
                            }
                            else Console.WriteLine("Cannot reject: Order is not Pending.");
                            break;

                        case "S":
                            if (o.OrderStatus == "Cancelled")
                            {
                                Console.WriteLine($"Order {o.OrderId} skipped. Status: {o.OrderStatus}");
                                validAction = true;
                            }
                            else Console.WriteLine("Cannot skip: Order is not Cancelled.");
                            break;

                        case "D":
                            if (o.OrderStatus == "Preparing")
                            {
                                o.OrderStatus = "Delivered";
                                Console.WriteLine($"Order {o.OrderId} delivered. Status: {o.OrderStatus}");
                                validAction = true;
                            }
                            else Console.WriteLine("Cannot deliver: Order is not Preparing.");
                            break;

                        default:
                            Console.WriteLine("Invalid choice.");
                            break;
                    }

                    Console.WriteLine(); // spacing

                    if (validAction) break; // exit the prompt loop for this order
                }
            }

            Console.WriteLine("\nAll orders processed for this restaurant.\n");
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
        // FEATURE 8 (BY: Chiam Sheng Le)
        // =========================
        static void DeleteOrder()
        {
            Console.Write("Enter Customer Email: ");
            string email = Console.ReadLine().Trim();

            Customer cust = customers.Find(c => c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (cust == null)
            {
                Console.WriteLine("Customer not found.");
                return;
            }

            // Get pending orders
            var pendingOrders = cust.OrderList.Where(o => o.OrderStatus == "Pending").ToList();

            if (!pendingOrders.Any())
            {
                Console.WriteLine("No pending orders found for this customer.");
                return;
            }

            Console.WriteLine("Pending Orders:");
            foreach (var o in pendingOrders)
            {
                Console.WriteLine(o.OrderId);
            }

            Console.Write("Enter Order ID: ");
            if (!int.TryParse(Console.ReadLine(), out int orderId))
            {
                Console.WriteLine("Invalid Order ID.");
                return;
            }

            Order orderToDelete = pendingOrders.Find(o => o.OrderId == orderId);
            if (orderToDelete == null)
            {
                Console.WriteLine("Order not found in pending orders.");
                return;
            }

            // Display basic order information
            Console.WriteLine($"\nCustomer: {cust.Name}");
            Console.WriteLine("Ordered Items:");
            int count = 1;
            foreach (var item in orderToDelete.OrderedItems)
            {
                Console.WriteLine($"{count}. {item.ItemName} - {item.QtyOrdered}");
                count++;
            }
            Console.WriteLine($"Delivery date/time: {orderToDelete.DeliveryDateTime:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Total Amount: ${orderToDelete.TotalAmount:F2}");
            Console.WriteLine($"Order Status: {orderToDelete.OrderStatus}");

            // Confirm deletion
            Console.Write("Confirm deletion? [Y/N]: ");
            string confirm = Console.ReadLine().Trim().ToUpper();

            if (confirm == "Y")
            {
                orderToDelete.OrderStatus = "Cancelled";
                refundStack.Push(orderToDelete);
                Console.WriteLine($"Order {orderToDelete.OrderId} cancelled. Refund of ${orderToDelete.TotalAmount:F2} processed.");
            }
            else
            {
                Console.WriteLine("Order deletion cancelled.");
            }

            Console.WriteLine(); // spacing
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

        // =========================
        // ADVANCED FEATURE B (BY: Chiam Sheng Le)
        // =========================
        static void DisplayTotalOrderAmount()
        {
            const double deliveryFee = 5.0;
            const double gruberooFeePercent = 0.3;

            double grandTotalOrders = 0;
            double grandTotalRefunds = 0;
            double totalGruberooEarnings = 0;

            foreach (var r in restaurants)
            {
                var deliveredOrders = r.OrderQueue.Where(o => o.OrderStatus == "Delivered").ToList();
                var refundedOrders = r.OrderQueue.Where(o => o.OrderStatus == "Rejected" || o.OrderStatus == "Cancelled").ToList();

                double restaurantTotal = deliveredOrders.Sum(o => o.TotalAmount - deliveryFee);
                double restaurantRefunds = refundedOrders.Sum(o => o.TotalAmount);
                double restaurantGruberoo = restaurantTotal * gruberooFeePercent;

                Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");
                Console.WriteLine($"Delivered Orders Total: ${restaurantTotal:F2}");
                Console.WriteLine($"Refunded Orders Total:  ${restaurantRefunds:F2}");
                Console.WriteLine($"Gruberoo Earnings:      ${restaurantGruberoo:F2}\n");

                grandTotalOrders += restaurantTotal;
                grandTotalRefunds += restaurantRefunds;
                totalGruberooEarnings += restaurantGruberoo;
            }

            Console.WriteLine("===== Summary =====");
            Console.WriteLine($"Total Orders Amount: ${grandTotalOrders:F2}");
            Console.WriteLine($"Total Refunds:       ${grandTotalRefunds:F2}");
            Console.WriteLine($"Final Earnings:      ${totalGruberooEarnings:F2}");
            Console.WriteLine("====================\n");

        }


    }
}