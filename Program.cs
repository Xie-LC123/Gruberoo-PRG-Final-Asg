//==========================================================
// Student Number : S10271327
// Student Name : Xie Liangchen
// Partner Name : Sheng Le
//==========================================================


// Feature 2
using PRG_Final_ASG;

namespace Gruberoo
{
    class Program
    {
        static List<Customer> customers = new List<Customer>();
        static List<Restaurant> restaurants = new List<Restaurant>();

        static void Main(string[] args)
        {
            LoadCustomers();
            LoadOrders();

            Console.WriteLine($"{customers.Count} customers loaded!");
            Console.WriteLine("Orders loaded successfully!");

            Console.ReadKey();
        }

        static void LoadCustomers()
        {
            string[] lines = File.ReadAllLines("customers.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');

                string name = data[0].Trim();
                string email = data[1].Trim();

                Customer customer = new Customer(name, email);
                customers.Add(customer);
            }
        }

        static void LoadOrders()
        {
            string[] lines = File.ReadAllLines("orders - Copy.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');

                int orderId = int.Parse(data[0].Trim());
                string customerEmail = data[1].Trim();
                string restaurantId = data[2].Trim();
                double totalAmount = double.Parse(data[4].Trim());
                string status = data[5].Trim();

                Order order = new Order(orderId, totalAmount, status);

                Customer customer = customers.Find(c => c.Email == customerEmail);

                if (customer != null)
                {
                    customer.AddOrder(order);
                }

                Restaurant restaurant = restaurants.Find(r => r.RestaurantId == restaurantId);

                if (restaurant != null)
                {
                    restaurant.AddOrderToQueue(order);
                }
            }
        }
    }

}

// Feature 3
static void ListAllRestaurants(List<Restaurant> restaurants)
{
    Console.WriteLine("All Restaurants and Menu Items");
    Console.WriteLine("==============================");

    foreach (Restaurant r in restaurants)
    {
        Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");

        foreach (Menu m in r.MenuList)
        {
            foreach (FoodItem f in m.FoodItems)
            {
                Console.WriteLine($" - {f.ItemName}: {f.Description} - ${f.Price:F2}");
            }
        }

        Console.WriteLine();
    }
}

// Feature 5
static void CreateOrder(List<Customer> customers, List<Restaurant> restaurants)
{
    Console.Write("Enter Customer Email: ");
    string email = Console.ReadLine();

    Customer cust = customers.Find(c => c.EmailAddress == email);
    if (cust == null)
    {
        Console.WriteLine("Invalid customer!");
        return;
    }

    Console.Write("Enter Restaurant ID: ");
    string restId = Console.ReadLine();

    Restaurant rest = restaurants.Find(r => r.RestaurantId == restId);
    if (rest == null)
    {
        Console.WriteLine("Invalid restaurant!");
        return;
    }

    Order order = new Order();
    order.OrderDateTime = DateTime.Now;

    Console.Write("Enter Delivery Date (dd/mm/yyyy): ");
    string date = Console.ReadLine();

    Console.Write("Enter Delivery Time (hh:mm): ");
    string time = Console.ReadLine();

    order.DeliveryDateTime = DateTime.Parse($"{date} {time}");

    Console.Write("Enter Delivery Address: ");
    order.DeliveryAddress = Console.ReadLine();

    // Display food items
    List<FoodItem> foodList = rest.GetAllFoodItems();
    int choice;

    do
    {
        for (int i = 0; i < foodList.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {foodList[i].ItemName} - ${foodList[i].Price}");
        }

        Console.Write("Enter item number (0 to finish): ");
        choice = int.Parse(Console.ReadLine());

        if (choice > 0 && choice <= foodList.Count)
        {
            Console.Write("Enter quantity: ");
            int qty = int.Parse(Console.ReadLine());

            OrderedFoodItem ofi = new OrderedFoodItem(foodList[choice - 1], qty);
            order.AddOrderedFoodItem(ofi);
        }

    } while (choice != 0);

    double total = order.CalculateOrderTotal();
    total += 5.00; // delivery fee

    Console.WriteLine($"Order Total: ${total:F2}");

    Console.Write("Proceed to payment? [Y/N]: ");
    string pay = Console.ReadLine().ToUpper();

    if (pay == "Y")
    {
        Console.Write("Payment Method [CC/PP/CD]: ");
        order.PaymentMethod = Console.ReadLine();

        order.OrderStatus = "Pending";
        order.OrderId = GenerateNewOrderId();

        cust.AddOrder(order);
        rest.AddOrderToQueue(order);

        Console.WriteLine($"Order {order.OrderId} created successfully! Status: Pending");
    }
}

// Feature 7
static void ModifyOrder(List<Customer> customers)
{
    Console.Write("Enter Customer Email: ");
    string email = Console.ReadLine();

    Customer cust = customers.Find(c => c.EmailAddress == email);
    if (cust == null)
    {
        Console.WriteLine("Invalid customer!");
        return;
    }

    var pendingOrders = cust.OrderList
                            .Where(o => o.OrderStatus == "Pending")
                            .ToList();

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders found.");
        return;
    }

    Console.WriteLine("Pending Orders:");
    foreach (var o in pendingOrders)
    {
        Console.WriteLine(o.OrderId);
    }

    Console.Write("Enter Order ID: ");
    int id = int.Parse(Console.ReadLine());

    Order order = pendingOrders.Find(o => o.OrderId == id);
    if (order == null)
    {
        Console.WriteLine("Invalid Order ID!");
        return;
    }

    Console.WriteLine("Modify: [1] Items [2] Address [3] Delivery Time");
    string choice = Console.ReadLine();

    double oldTotal = order.CalculateOrderTotal();

    if (choice == "2")
    {
        Console.Write("Enter new address: ");
        order.DeliveryAddress = Console.ReadLine();
    }
    else if (choice == "3")
    {
        Console.Write("Enter new time (hh:mm): ");
        string newTime = Console.ReadLine();

        order.DeliveryDateTime =
            new DateTime(order.DeliveryDateTime.Year,
                         order.DeliveryDateTime.Month,
                         order.DeliveryDateTime.Day,
                         int.Parse(newTime.Split(':')[0]),
                         int.Parse(newTime.Split(':')[1]),
                         0);
    }

    double newTotal = order.CalculateOrderTotal();

    if (newTotal > oldTotal)
    {
        Console.WriteLine("Additional payment required.");
        Console.Write("Proceed? [Y/N]: ");
        if (Console.ReadLine().ToUpper() != "Y")
            return;
    }

    Console.WriteLine($"Order {order.OrderId} updated successfully.");
}

// Advanced Feature A
static void BulkProcessOrders(List<Restaurant> restaurants)
{
    Console.WriteLine("Bulk Processing of Pending Orders (Today)");
    Console.WriteLine("==========================================");

    DateTime now = DateTime.Now;

    int totalOrders = 0;
    int pendingCount = 0;
    int processedCount = 0;
    int preparingCount = 0;
    int rejectedCount = 0;

    // First count ALL orders in system
    foreach (Restaurant r in restaurants)
    {
        totalOrders += r.OrderQueue.Count;

        foreach (Order o in r.OrderQueue)
        {
            if (o.OrderStatus == "Pending")
                pendingCount++;
        }
    }

    Console.WriteLine($"Total Pending Orders: {pendingCount}");
    Console.WriteLine();

    // Process pending orders
    foreach (Restaurant r in restaurants)
    {
        foreach (Order o in r.OrderQueue)
        {
            if (o.OrderStatus == "Pending")
            {
                processedCount++;

                TimeSpan timeDiff = o.DeliveryDateTime - now;

                if (timeDiff.TotalMinutes < 60)
                {
                    o.OrderStatus = "Rejected";
                    rejectedCount++;
                }
                else
                {
                    o.OrderStatus = "Preparing";
                    preparingCount++;
                }
            }
        }
    }

    Console.WriteLine("Processing Summary");
    Console.WriteLine("==================");
    Console.WriteLine($"Orders Processed: {processedCount}");
    Console.WriteLine($"Preparing Orders: {preparingCount}");
    Console.WriteLine($"Rejected Orders: {rejectedCount}");

    double percentage = 0;
    if (totalOrders > 0)
        percentage = (double)processedCount / totalOrders * 100;

    Console.WriteLine($"Auto-processed Percentage: {percentage:F2}%");
}
