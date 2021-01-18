using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SkyblockAuctionTracker.ApiServices;
using SkyblockAuctionTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SkyblockAuctionTracker.ApiServices.BazaarResponse;

namespace SkyblockAuctionTracker.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BazaarController : ControllerBase
    {
        private readonly ILogger<BazaarController> logger;
        private readonly ISkyblockApiService skyblockApiService;
        private readonly AMCDbContext db;
        private long lastUpdated = 0;
        public BazaarController(ISkyblockApiService service, AMCDbContext context, ILogger<BazaarController> logger)
        {
            skyblockApiService = service;
            db = context;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await skyblockApiService.GetBazaarResponse();

            if (response.Success == false)
            {
                return BadRequest("Bazaar unavailble");
            }

            if (lastUpdated != response.LastUpdated)
            {
                lastUpdated = response.LastUpdated;

                long timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                foreach (var product in response.Products)
                {
                    try
                    {
                        BazaarProductIDMapping mapping = db.BazaarProductIDMappings.FirstOrDefault(e => e.Name == product.Key);
                        if (mapping == null)
                        {
                            logger.LogError("Mapping was NULL");
                            continue;
                        }

                        BazaarProductInfo info = product.Value.BazaarProductInfo;

                        db.BazaarProductEntries.Add(new BazaarProductEntry()
                        {
                            Timestamp = timestamp,
                            MappingID = mapping.ID, 
                            BuyPrice = info.BuyPrice, 
                            SellPrice = info.SellPrice 
                        });
                    }
                    catch(Exception e)
                    {
                        logger.LogError(e, "Failed adding bazaar entry");
                    }
                }

                try
                {
                    db.SaveChanges();
                } 
                catch(Exception e)
                {
                    logger.LogError(e, "Failed to save bazaar data");
                }
            } 
            else
            {
                return BadRequest("Already up-to-date");
            }

            // I used this to easily get seed data for bazaar product mappings 
            /*
            string teststring = "";
            int counter = 0;
            foreach (var product in response.Products)
            {
                teststring += "new BazaarProductIDMapping { ID = " + counter.ToString() + ", Name = \"" + product.Key.ToString() + "\" },\r\n";
                counter++;
            }
            */

            return Ok("OK");
        }
    }
}
