using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class Restaurant
    {
        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantStatus { get; set; }

        public Restaurant(string restaurantId, string restaurantName, string restaurantStatus)
        {
            RestaurantId=restaurantId;
            RestaurantName=restaurantName;
            RestaurantStatus=restaurantStatus;
        }
        private List<Menu> menus = new List<Menu>();
        private List<Order> orders = new List<Order>();
        private List<SpecialOffer> specialOffers = new List<SpecialOffer>();

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
            foreach (var menu in menus)
            {
                System.Console.WriteLine(menu);
            }
        }

        public void DisplayOrders()
        {
            foreach (var order in orders)
            {
                System.Console.WriteLine(order);
            }
        }

        public void DisplaySpecialOffers()
        {
            foreach (var offer in specialOffers)
            {
                System.Console.WriteLine(offer);
            }
        }

        public override string ToString()
        {
            return RestaurantName;
        }

    }
}
