using System;

namespace eTilbudsharp.Api
{

    public class Offer
    {
        public string id { get; set; }
        public string ern { get; set; }
        public string heading { get; set; }
        public string description { get; set; }
        public int catalog_page { get; set; }
        public Pricing pricing { get; set; }
        public Quantity quantity { get; set; }
        public Images images { get; set; }
        public Links links { get; set; }
        public DateTime run_from { get; set; }
        public DateTime run_till { get; set; }
        public DateTime publish { get; set; }
        public string dealer_url { get; set; }
        public string catalog_url { get; set; }
        public string store_url { get; set; }
        public string store_id { get; set; }
        public string dealer_id { get; set; }
        public string catalog_id { get; set; }
    }
}