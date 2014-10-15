using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Rating
    {
        public readonly User User;
        public decimal Rating { get; set; }
        public readonly Recipe Recipe;
    }
}