using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkyblockAuctionTracker.ApiServices
{
    public class BazaarResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("lastUpdated")]
        public long LastUpdated { get; set; }
        [JsonPropertyName("products")]
        public Dictionary<string, BazaarProduct> Products { get; set; }

        public class BazaarProduct
        {
            [JsonPropertyName("product_id")]
            public string ID { get; set; }
            [JsonPropertyName("quick_status")]
            public BazaarProductInfo BazaarProductInfo { get; set; }
        }

        public class BazaarProductInfo
        {
            [JsonPropertyName("sellPrice")]
            public double SellPrice { get; set; }
            [JsonPropertyName("sellVolume")]
            public long SellVolume { get; set; }
            [JsonPropertyName("sellMovingWeek")]
            public long SellMovingWeek { get; set; }
            [JsonPropertyName("sellOrders")]
            public long SellOrders { get; set; }
            [JsonPropertyName("buyPrice")]
            public double BuyPrice { get; set; }
            [JsonPropertyName("buyVolume")]
            public long BuyVolume { get; set; }
            [JsonPropertyName("buyMovingWeek")]
            public long BuyMovingWeek { get; set; }
            [JsonPropertyName("buyOrders")]
            public long BuyOrders { get; set; }
        }
    }
}
