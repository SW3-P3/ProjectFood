using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Group
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
    }
}