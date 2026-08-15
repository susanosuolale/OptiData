using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OptiData.Infrastructure.Data;
using OptiData.Domain.Entities;

namespace OptiData.Infrastructure.Jobs
{
    // What is the point of the Data Usage Simulator?
    // In a real enterprise app, we would have a background job that connects directly to the telecom provider
    // (like MTN or Airtel) and downloads the user's live data usage every 5 minutes.
    // Since we don't have access to live telecom APIs for this portfolio, this simulator fakes that process.
    // It automatically deducts data and creates usage records every 5 minutes, giving the Machine Learning
    // model an active, growing dataset to analyze.
    public class DataUsageSimulatorJob
    {
        private readonly AppDbContext _context;

        public DataUsageSimulatorJob(AppDbContext context)
        {
            _context = context;
        }

        public async Task SimulateUsageAsync()
        {
            // 1. Find the Test User in the database
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == DataSeeder.TestUserId);
            
            if (user != null)
            {
                var random = new Random();
                // Real-world Nigerian stats: ~14.8 GB per month = ~1.75 MB every 5 minutes.
                // We randomly generate between 1MB and 3MB every 5 minutes to average ~15GB/month.
                var megabytesUsed = random.Next(1, 4);

                // 2. Deduct the usage from the user's current data balance
                user.CurrentBalanceMB -= megabytesUsed;
                if (user.CurrentBalanceMB < 0)
                {
                    user.CurrentBalanceMB = 0;
                }

                // 3. Create a new record of this usage and save it to the database
                var usageRecord = new UsageRecord
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    DataConsumedMB = megabytesUsed,
                    Timestamp = DateTime.UtcNow
                };

                _context.UsageRecords.Add(usageRecord);
                
                // Save both the new balance and the new record at the same time
                await _context.SaveChangesAsync();
            }
        }
    }
}
