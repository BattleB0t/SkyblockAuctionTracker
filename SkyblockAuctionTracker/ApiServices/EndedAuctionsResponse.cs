using fNbt;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SkyblockAuctionTracker.ApiServices
{
    public class EndedAuctionsResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
        [JsonPropertyName("lastUpdated")]
        public long LastUpdated { get; set; }
        [JsonPropertyName("auctions")]
        public List<EndedAuctionResult> AuctionResults { get; set; }

        public class EndedAuctionResult
        {
            private string itemBytes;

            [JsonPropertyName("auction_id")]
            public string AuctionID { get; set; }
            [JsonPropertyName("seller")]
            public string SellerUUID { get; set; }
            [JsonPropertyName("seller_profile")]
            public string SellerProfileID { get; set; }
            [JsonPropertyName("buyer")]
            public string BuyerUUID { get; set; }
            [JsonPropertyName("timestamp")]
            public long Timestamp { get; set; }
            [JsonPropertyName("price")]
            public long Price { get; set; }
            [JsonPropertyName("bin")]
            public bool BIN { get; set; }
            [JsonPropertyName("item_bytes")]
            public string ItemBytes 
            { 
                get { return itemBytes; } 
                set 
                { 
                    var enc = Convert.FromBase64String(value);
                    
                    using (var compressedStream = new MemoryStream(enc))
                    using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
                    using (var resultStream = new MemoryStream())
                    {
                        zipStream.CopyTo(resultStream);
                        enc = resultStream.ToArray();
                    }

                    var myFile = new NbtFile();
                    myFile.LoadFromBuffer(enc, 0, enc.Length, NbtCompression.None);
                    var myCompoundTag = myFile.RootTag;
                    int test = myCompoundTag["i"][0]["id"].IntValue;

                    string S1 = myCompoundTag["i"][0]["tag"]["display"]["Lore"][0].StringValue;
                    string S2 = myCompoundTag["i"][0]["tag"]["display"]["Lore"][1].StringValue;
                    string S3 = myCompoundTag["i"][0]["tag"]["display"]["Lore"][2].StringValue;
                    string S4 = myCompoundTag["i"][0]["tag"]["display"]["Lore"][3].StringValue;

                    Console.WriteLine(myCompoundTag.ToString());

                    itemBytes = System.Text.Encoding.ASCII.GetString(enc);
                } 
            }
        }
    }
}
