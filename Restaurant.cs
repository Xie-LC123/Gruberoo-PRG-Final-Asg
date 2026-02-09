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

        public Restaurant(string id, string name, string email)
        {
            restaurantId = id;
            restaurantName = name;
            restaurantEmail = email;
            menus = new List<Menu>();
            specialOffers = new List<SpecialOffer>();
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

        public IEnumerable<Order> OrderQueue { get; internal set; }
    }
}
