using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gruberoo
{
    internal class SpecialOffer
    {
        public string OfferCode { get; set; }
        public string OfferDesc { get; set; }
        public double Discount { get; set; }

        public SpecialOffer(string offerCode, string offerDesc, double discount)
        {
            OfferCode=offerCode;
            OfferDesc=offerDesc;
            Discount=discount;
        }
        public override string ToString()
        {
            return $"{OfferCode}: {OfferDesc} (-${Discount})";
        }
    }
}
