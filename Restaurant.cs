using System;
using System.Collections.Generic;
using System.Text;
using Gruberoo;

namespace PRG_Final_ASG
{
    internal class Restaurant
    {
        private string restaurantId;
        private string restaurantName;
        private string restaurantEmail;

        private List<Menu> menus;
        private List<SpecialOffer> specialOffers;

        // Add a private field for the order queue
        private List<Order> orderQueue;

        public Restaurant(string id, string name, string email)
        {
            restaurantId = id;
            restaurantName = name;
            restaurantEmail = email;
            menus = new List<Menu>();
            specialOffers = new List<SpecialOffer>();
            orderQueue = new List<Order>(); // Initialize the order queue
        }

        public void AddMenu(Menu menu)
        {
            menus.Add(menu);
        }

        public bool RemoveMenu(Menu menu)
        {
            return menus.Remove(menu);
        }

        public void DisplayMenu()
        {
            foreach (Menu menu in menus)
            {
                Console.WriteLine(menu);
                menu.DisplayFoodItems();
            }
        }

        public void AddSpecialOffer(SpecialOffer offer)
        {
            specialOffers.Add(offer);
        }

        public void DisplaySpecialOffers()
        {
            foreach (SpecialOffer offer in specialOffers)
            {
                Console.WriteLine(offer);
            }
        }

        public override string ToString()
        {
            return $"{restaurantName} ({restaurantId}) - {restaurantEmail}";
        }

        public string RestaurantId => restaurantId;
        public string RestaurantName => restaurantName;

        // Update OrderQueue property to expose the private field
        public IEnumerable<Order> OrderQueue => orderQueue;

        public IEnumerable<FoodItem> FoodItems { get; internal set; }

        public void AddFoodItem(FoodItem item)
        {
            // Assuming FoodItems is a List<FoodItem> internally
            var foodItemsList = FoodItems as List<FoodItem>;
            if (foodItemsList != null)
                foodItemsList.Add(item);
        }

        public void AddOrderToQueue(Order order)
        {
            orderQueue.Add(order);
        }
    }
}
