using System;
using System.ComponentModel.DataAnnotations;

namespace ProjectFood.Models
{
    public static class GlobalVariables
    {
        [DataType(DataType.Date)]
        public static DateTime CurrentSystemTime { get; set; }
    }
}