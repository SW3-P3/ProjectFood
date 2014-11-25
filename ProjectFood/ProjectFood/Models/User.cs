﻿using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public List<Group> Groups { get; set; }
        public List<ShoppingList> ShoppingLists { get; set; }
        public virtual ShoppingList WatchList { get; set; }
        public virtual ShoppingList RelevantOffers { get; set; }
        public List<Pref> Preferences { get; set; }
        public List<Rating> Ratings { get; set; }
    }
}