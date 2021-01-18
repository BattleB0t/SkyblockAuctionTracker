using Microsoft.Extensions.Logging;
using Quartz;
using SkyblockAuctionTracker.ApiServices;
using SkyblockAuctionTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static SkyblockAuctionTracker.ApiServices.BazaarResponse;

namespace SkyblockAuctionTracker.Schedules
{
    public class BazaarScheduleJob : IJob
    {
        private readonly ISkyblockApiService skyblockApiService;
        private readonly AMCDbContext db;
        private readonly ILogger<BazaarScheduleJob> logger;

        private long lastUpdated = 0;

        public BazaarScheduleJob(ISkyblockApiService service, AMCDbContext context, ILogger<BazaarScheduleJob> logger)
        {
            skyblockApiService = service;
            db = context;
            this.logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            int warnings = 0;
            int errors = 0;

            logger.LogInformation($"[{DateTimeOffset.Now}]BazaarScheduleJob started");

            // Get bazaar api data
            var response = await skyblockApiService.GetBazaarResponse();

            if (response.Success == false)
            {
                // Request failed
                errors++;
                // TODO
            }

            if (lastUpdated != response.LastUpdated && errors == 0)
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
                            errors++;
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
                    catch (Exception e)
                    {
                        errors++;
                        logger.LogError(e, "Failed adding bazaar entry");
                    }
                }

                try
                {
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    errors++;
                    logger.LogError(e, "Failed to save bazaar data");
                }
            }
            else
            {
                warnings++;
                logger.LogWarning($"[{DateTimeOffset.Now}]Bazaar already up-to-date");
            }

            logger.LogInformation($"[{DateTimeOffset.Now}]BazaarScheduleJob finished with {warnings} warnings and {errors} errors");
        }
    }
}