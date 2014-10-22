using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eTilbudsharp.Api
{
    public class Store
    {
        public string id { get; set; }
        public string ern { get; set; }
        public string street { get; set; }
        public string city { get; set; }
        public string zip_code { get; set; }
        public Country country { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string dealer_url { get; set; }
        public string dealer_id { get; set; }
        public Branding branding { get; set; }
        public string contact { get; set; }
        public List<string> category_ids { get; set; }
    }

}
