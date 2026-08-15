using System;
using Hangfire;
using OptiData.Application.Interfaces;
using OptiData.Domain.Enums;
using OptiData.Infrastructure.Jobs;

namespace OptiData.Infrastructure.Services
{
    public class HangfirePurchaseScheduler : IPurchaseSchedulerService
    {
        public void SchedulePurchase(Guid userId, decimal predictedMB, DataProvider provider, int hoursToWait)
        {
            BackgroundJob.Schedule<BundlePurchaseJob>(
                // what job to execute
                job => job.ExecutePurchaseSimulationAsync(userId, predictedMB, provider),
                // tells background job when to start executing
                TimeSpan.FromHours(hoursToWait)
            );
        }
    }
}
