using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Rating
    {
        public readonly User User;
        public decimal Score { get; set; }
        public readonly Recipe Recipe;
    }
}