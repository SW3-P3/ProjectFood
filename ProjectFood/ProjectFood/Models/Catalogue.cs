using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Catalogue
    {
        public readonly Store Store;
        public readonly DateTime Begin;
        public readonly DateTime End;
        public List<Offer> Offers { get; set; }
    }
}