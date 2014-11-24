using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Rating
    {
        public virtual User User { get; set; }
        public decimal Score { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}