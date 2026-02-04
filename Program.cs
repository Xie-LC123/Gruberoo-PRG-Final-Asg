//==========================================================
// Student Number : S10271327
// Student Name : Xie Liangchen
// Partner Name : Sheng Le
//==========================================================

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

