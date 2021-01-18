using Microsoft.Extensions.Logging;
using Quartz;
using SkyblockAuctionTracker.ApiServices;
using SkyblockAuctionTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SkyblockAuctionTracker.Schedules
{
    public class BazaarSchedule : IJob
    {
        private readonly ISkyblockApiService skyblockApiService;
        private readonly AMCDbContext db;
        private readonly ILogger<BazaarSchedule> logger;

        public BazaarSchedule(ISkyblockApiService service, AMCDbContext context, ILogger<BazaarSchedule> logger)
        {
            skyblockApiService = service;
            db = context;
            this.logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            logger.LogInformation("Bazaar Job!");
            return Task.CompletedTask;
        }
    }
}
