using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;

namespace OptiData.Infrastructure.Data
{
    // What is the point of the Data Seeder?
    // In a real enterprise app, users naturally generate history over months.
    // For a portfolio, if the database starts empty, the Machine Learning model has nothing to learn from and returns 0.
    // This seeder runs at startup to automatically inject 50 fake historical records for a "Test User" 
    // so the ML model works instantly for recruiters testing the app.
    public static class DataSeeder
    {
        // We use a permanent fake ID so the entire application knows who the "logged in" user is.
        public static readonly Guid TestUserId = Guid.Parse("00000000-0000-0000-0000-000000000001");

        public static async Task SeedAsync(AppDbContext context)
        {
            // 1. Create the Test User if they don't exist
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == TestUserId);
            if (user == null)
            {
                user = new User
                {
                    Id = TestUserId,
                    Name = "John Doe Recruiter",
                    CurrentBalanceMB = 5000, // Starting balance of 5GB
                    PreferredProvider = DataProvider.MTN
                };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // 2. Generate Fake Historical Usage Records for the ML model to learn from
            var usageCount = await context.UsageRecords.CountAsync(u => u.UserId == TestUserId);
            if (usageCount < 700)
            {
                var random = new Random();
                var historicalRecords = new List<UsageRecord>();

                // Real-world Nigerian stats: The average user consumes ~14.8 GB per month.
                // That is ~500 MB per day, or ~20 MB per hour.
                // We will seed 1 record for every single hour of the past 30 days (720 records)
                // so the ML model has a perfectly smooth and realistic dataset to learn from.
                for (int hoursAgo = 720; hoursAgo > 0; hoursAgo--)
                {
                    var pastTime = DateTime.UtcNow.AddHours(-hoursAgo);

                    historicalRecords.Add(new UsageRecord
                    {
                        Id = Guid.NewGuid(),
                        UserId = TestUserId,
                        DataConsumedMB = random.Next(10, 35), // Averages ~22 MB per hour (~15.8 GB per month)
                        Timestamp = pastTime
                    });
                }

                context.UsageRecords.AddRange(historicalRecords);
                await context.SaveChangesAsync();
            }
        }
    }
}
