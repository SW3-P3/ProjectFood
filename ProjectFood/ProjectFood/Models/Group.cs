using System;
using System.Collections.Generic;

namespace ProjectFood.Models
{
    public class Group
    {
        public readonly int ID;
        public string Name { get; set; }
        public List<User> Users { get; set; }

        public Group(string name, User initialUser)
        {
            ID = 1; //fix later
            Name = (name != string.Empty) ? name : "some name";
            Users.Add(initialUser);
        }
    }
}