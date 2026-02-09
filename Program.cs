//==========================================================
// Student Number : S10271327
// Student Name : Xie Liangchen
// Student Number : S10273654
// Student Name : Chiam Sheng Le
//==========================================================


// Feature 1,2
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
            LoadRestaurants();
            LoadFoodItems();
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

        static void tLoadOrders()
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
        static void LoadFoodItems()
        {
            string[] lines = File.ReadAllLines("fooditems.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');

                string itemId = data[0].Trim();
                string itemName = data[1].Trim();
                double price = double.Parse(data[2].Trim());
                string restaurantId = data[3].Trim();

                FoodItem foodItem = new FoodItem(itemId, itemName, price);

                Restaurant restaurant = restaurants.Find(r => r.RestaurantId == restaurantId);

                if (restaurant != null)
                {
                    restaurant.AddFoodItem(foodItem);
                }
            }
        }
        static void LoadRestaurants()
        {
            string[] lines = File.ReadAllLines("restaurants.csv");

            for (int i = 1; i < lines.Length; i++)
            {
                string[] data = lines[i].Split(',');

                string restaurantId = data[0].Trim();
                string restaurantName = data[1].Trim();

                Restaurant restaurant = new Restaurant(restaurantId, restaurantName);
                restaurants.Add(restaurant);
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
// Feature 4

static void ListAllOrders(List<Order> orders)
{
    Console.WriteLine("\nAll Orders");
    Console.WriteLine("==========");

    Console.WriteLine(
        "Order ID   Customer        Restaurant        Delivery Date/Time     Amount     Status"
    );

    foreach (Order o in orders)
    {
        Console.WriteLine(
            $"{o.OrderID,-10} {o.Customer.Name,-15} {o.Restaurant.Name,-17} " +
            $"{o.DeliveryDateTime:dd/MM/yyyy HH:mm}   ${o.TotalAmount,-8:F2} {o.Status}"
        );
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

// Feature 6
static void ProcessOrder(
    List<Restaurant> restaurants,
    Stack<Order> refundStack)
{
    Console.Write("\nEnter Restaurant ID: ");
    string restID = Console.ReadLine();

    Restaurant r = restaurants.Find(x => x.RestaurantID == restID);

    if (r == null)
    {
        Console.WriteLine("Invalid Restaurant ID.");
        return;
    }

    foreach (Order o in r.OrderQueue)
    {
        Console.WriteLine($"\nOrder {o.OrderID}:");
        Console.WriteLine($"Customer: {o.Customer.Name}");
        Console.WriteLine("Ordered Items:");

        foreach (OrderedFoodItem item in o.OrderedItems)
        {
            Console.WriteLine($"- {item.ItemName} x {item.QtyOrdered}");
        }

        Console.WriteLine($"Delivery: {o.DeliveryDateTime}");
        Console.WriteLine($"Total Amount: ${o.TotalAmount:F2}");
        Console.WriteLine($"Status: {o.Status}");

        Console.Write("[C]onfirm / [R]eject / [S]kip / [D]eliver: ");
        string choice = Console.ReadLine().ToUpper();

        switch (choice)
        {
            case "C":
                if (o.Status == "Pending")
                {
                    o.Status = "Preparing";
                    Console.WriteLine("Order confirmed. Status: Preparing");
                }
                break;

            case "R":
                if (o.Status == "Pending")
                {
                    o.Status = "Rejected";
                    refundStack.Push(o);
                    Console.WriteLine("Order rejected. Refund processed.");
                }
                break;

            case "S":
                if (o.Status == "Cancelled")
                {
                    Console.WriteLine("Order skipped.");
                }
                break;

            case "D":
                if (o.Status == "Preparing")
                {
                    o.Status = "Delivered";
                    Console.WriteLine("Order delivered successfully.");
                }
                break;

            default:
                Console.WriteLine("Invalid option.");
                break;
        }
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

// Feature 8
static void DeleteOrder(Stack<Order> refundStack, List<Customer> customers)
{
    Console.Write("\nEnter Customer Email: ");
    string email = Console.ReadLine();

    Customer c = customers.Find(x => x.Email == email);

    if (c == null)
    {
        Console.WriteLine("Invalid customer.");
        return;
    }

    var pendingOrders = c.OrderList.FindAll(o => o.Status == "Pending");

    if (pendingOrders.Count == 0)
    {
        Console.WriteLine("No pending orders.");
        return;
    }

    Console.WriteLine("Pending Orders:");
    foreach (Order o in pendingOrders)
    {
        Console.WriteLine(o.OrderID);
    }

    Console.Write("Enter Order ID: ");
    int orderID = int.Parse(Console.ReadLine());

    Order order = pendingOrders.Find(o => o.OrderID == orderID);

    if (order == null)
    {
        Console.WriteLine("Invalid Order ID.");
        return;
    }

    Console.WriteLine($"\nCustomer: {order.Customer.Name}");
    Console.WriteLine($"Total Amount: ${order.TotalAmount:F2}");
    Console.WriteLine($"Status: {order.Status}");

    Console.Write("Confirm deletion? [Y/N]: ");
    if (Console.ReadLine().ToUpper() == "Y")
    {
        order.Status = "Cancelled";
        refundStack.Push(order);

        Console.WriteLine(
            $"Order {order.OrderID} cancelled. Refund of ${order.TotalAmount:F2} processed."
        );
    }
}
