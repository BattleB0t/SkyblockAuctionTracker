using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SkyblockAuctionTracker.ApiServices
{
    public class SkyblockApiServiceOptions
    {
        public string ApiToken { get; set; }
    }
    public class SkyblockApiService : ISkyblockApiService
    {
        private string apiToken = "";
        private const string baseUrl = "https://api.hypixel.net/skyblock/";

        private HttpClient client;
        private readonly ILogger<SkyblockApiService> logger;

        public SkyblockApiService(HttpClient client, ILogger<SkyblockApiService> logger, IOptions<SkyblockApiServiceOptions> options)
        {
            apiToken = options.Value.ApiToken;
            this.client = client;
            this.logger = logger;
        }

        public async Task<EndedAuctionsResponse> GetEndedAuctions()
        {
            try
            {
                using var responseStream = await client.GetStreamAsync($"{baseUrl}auctions_ended?key={apiToken}");
                var result = await JsonSerializer.DeserializeAsync<EndedAuctionsResponse>(responseStream);

                return result;
            } 
            catch(Exception e)
            {
                logger.LogError(e, "Error calling skyblock api");
                return null;
            }
        }

        public async Task<BazaarResponse> GetBazaarResponse()
        {
            try
            {
                using var responseStream = await client.GetStreamAsync($"{baseUrl}bazaar?key={apiToken}");
                var result = await JsonSerializer.DeserializeAsync<BazaarResponse>(responseStream);

                return result;
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error calling skyblock api");
                return null;
            }
        }
    }
}
