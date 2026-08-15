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
            if (hoursToWait <= 0)
            {
                // Enqueue instantly bypasses Hangfire's 15-second schedule poller delay
                BackgroundJob.Enqueue<BundlePurchaseJob>(
                    job => job.ExecutePurchaseSimulationAsync(userId, predictedMB, provider)
                );
            }
            else
            {
                BackgroundJob.Schedule<BundlePurchaseJob>(
                    job => job.ExecutePurchaseSimulationAsync(userId, predictedMB, provider),
                    TimeSpan.FromHours(hoursToWait)
                );
            }
        }
    }
}
