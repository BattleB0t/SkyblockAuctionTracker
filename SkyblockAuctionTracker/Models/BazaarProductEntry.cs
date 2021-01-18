using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SkyblockAuctionTracker.Models
{
    public class BazaarProductEntry
    {
        public long Timestamp { get; set; }

        [ForeignKey("BazaarProductIDMapping")]
        public int MappingID { get; set; }
        public double SellPrice { get; set; }
        public double BuyPrice { get; set; }
    }
}
