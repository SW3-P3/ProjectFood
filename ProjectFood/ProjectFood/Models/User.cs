using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class User
    {
        public string Username { get; set; }
        //something something password hash ..
        public string Name { get; set; }
        public List<Group> Groups { get; set; }
        public List<ShoppingList> ShoppingLists { get; set; }
        //something something præferencer
    }
}