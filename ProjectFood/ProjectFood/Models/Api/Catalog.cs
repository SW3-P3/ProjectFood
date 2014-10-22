using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectFood.Models.Api
{
    public class Catalog
    {
        public string id { get; set; }
        public string ern { get; set; }
        public object label { get; set; }
        public string background { get; set; }
        public DateTime run_from { get; set; }
        public DateTime run_till { get; set; }
        public int page_count { get; set; }
        public int offer_count { get; set; }
        public Branding branding { get; set; }
        public string dealer_id { get; set; }
        public string dealer_url { get; set; }
        public string store_id { get; set; }
        public string store_url { get; set; }
        public Dimensions dimensions { get; set; }
        public Images images { get; set; }
        public Pages pages { get; set; }
    }
}
