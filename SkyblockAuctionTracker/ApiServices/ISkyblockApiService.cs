using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkyblockAuctionTracker.ApiServices
{
    public interface ISkyblockApiService
    {
        Task<EndedAuctionsResponse> GetEndedAuctions();
        Task<BazaarResponse> GetBazaarResponse();
    }
}
