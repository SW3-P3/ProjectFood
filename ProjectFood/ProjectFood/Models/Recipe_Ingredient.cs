﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProjectFood.Models
{
    public class Recipe_Ingredient
    {
        [Key, Column(Order = 0)]
        public int RecipeID { get; set; }
        [Key, Column(Order = 1)]
        public int IngredientID { get; set; }
        public virtual Recipe Recipe { get; set; }
        public virtual Item Ingredient { get; set; }
        public double AmountPerPerson { get; set; }
        public string Unit { get; set; }
    }
}