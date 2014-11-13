using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace ProjectFood.Models
{
    public class Recipe
    {
        public int ID { get; set; }
        public int AuthorID { get; set; }
        public string AuthorName { get; set; }
        public string Title { get; set; }
        public ICollection<Item> Ingredients { get; set; }
        public string Tags { get; set; }
        public int Minutes { get; set; }
        public string Instructions { get; set; }

        public Recipe()
        {
            Ingredients = new List<Item>();
        }
    }
}