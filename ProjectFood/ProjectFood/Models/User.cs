﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace ProjectFood.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public virtual ICollection<ShoppingList> ShoppingLists { get; set; }
        public virtual ShoppingList WatchList { get; set; }
        public DateTime? LastSentNotification { get; set; }
        public int? MaxSendEmailsEveryDays { get; set; }
        public List<Offer> SentOffers { get; set; } 
        public virtual ShoppingList RelevantOffers { get; set; }
        public List<Pref> Preferences { get; set; }
        public List<Rating> Ratings { get; set; }

        public User()
        {
            ShoppingLists = new List<ShoppingList>();
            Preferences = new List<Pref>();
            Ratings = new List<Rating>();
            SentOffers = new List<Offer>();
            LastSentNotification = new DateTime(2014,01,01);
            MaxSendEmailsEveryDays = 1;
        }
    }
}