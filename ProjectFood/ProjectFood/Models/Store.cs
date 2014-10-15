using System;
using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    class Store
    {
        public string Chain { get; set; }
        public List<Offer> CurrentOffers { get; set; }
        public List<Offer> NextOffers { get; set; }
    }
}
