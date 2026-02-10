using System;
using System.Collections.Generic;

namespace PRG_Final_ASG
{
    internal class Restaurant
    {
        // =====================
        // Private fields
        // =====================
        private string restaurantId;
        private string restaurantName;
        private string restaurantEmail;

        private List<Menu> menus;
        private List<SpecialOffer> specialOffers;
        private List<Order> orderQueue;

        // =====================
        // Constructor
        // =====================
        public Restaurant(string id, string name, string email)
        {
            restaurantId = id;
            restaurantName = name;
            restaurantEmail = email;

            menus = new List<Menu>();
            specialOffers = new List<SpecialOffer>();
            orderQueue = new List<Order>();
            FoodItems = new List<FoodItem>(); // Important: initialize
        }

        // =====================
        // Properties
        // =====================
        public string RestaurantId => restaurantId;
        public string RestaurantName => restaurantName;
        public string RestaurantEmail => restaurantEmail;

        // Expose orders safely (read-only)
        public IEnumerable<Order> OrderQueue => orderQueue;

        // FoodItems list (internal add allowed)
        public List<FoodItem> FoodItems { get; private set; }

        // Menus and SpecialOffers read-only
        public IEnumerable<Menu> Menus => menus;
        public IEnumerable<SpecialOffer> SpecialOffers => specialOffers;

        // =====================
        // Methods
        // =====================
        public void AddFoodItem(FoodItem item)
        {
            if (item != null)
                FoodItems.Add(item);
        }

        public void AddOrderToQueue(Order order)
        {
            if (order != null)
                orderQueue.Add(order);
        }

        public void AddMenu(Menu menu)
        {
            if (menu != null)
                menus.Add(menu);
        }

        public bool RemoveMenu(Menu menu)
        {
            return menus.Remove(menu);
        }

        public void DisplayMenu()
        {
            if (menus.Count == 0)
            {
                Console.WriteLine("No menus available.");
                return;
            }

            foreach (var menu in menus)
            {
                Console.WriteLine(menu);
                menu.DisplayFoodItems();
            }
        }

        public void AddSpecialOffer(SpecialOffer offer)
        {
            if (offer != null)
                specialOffers.Add(offer);
        }

        public void DisplaySpecialOffers()
        {
            if (specialOffers.Count == 0)
            {
                Console.WriteLine("No special offers available.");
                return;
            }

            foreach (var offer in specialOffers)
                Console.WriteLine(offer);
        }

        // =====================
        // ToString override
        // =====================
        public override string ToString()
        {
            return $"{restaurantName} ({restaurantId}) - {restaurantEmail}";
        }
    }
}
