using System;
using System.Collections.Generic;
using System.Text;

namespace PRG_Final_ASG
{
    internal class SpecialOffer
    {
        private string offerCode;
        private string offerDesc;
        private double discount;

        public SpecialOffer(string code, string description, double discount)
        {
            offerCode = code;
            offerDesc = description;
            this.discount = discount;
        }

        public override string ToString()
        {
            if (discount > 0)
                return $"{offerCode}: {offerDesc} ({discount}% off)";
            else
                return $"{offerCode}: {offerDesc}";
        }

        public double Discount => discount;
    }
}
