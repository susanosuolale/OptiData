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
            // For Portfolio Demonstration: If hoursToWait is 0, it means we triggered an immediate demo purchase.
            // We wait exactly 10 seconds so the user can see the Toast message pop up in real-time.
            var delay = hoursToWait == 0 ? TimeSpan.FromSeconds(10) : TimeSpan.FromHours(hoursToWait);

            BackgroundJob.Schedule<BundlePurchaseJob>(
                // what job to execute
                job => job.ExecutePurchaseSimulationAsync(userId, predictedMB, provider),
                // tells background job when to start executing
                delay
            );
        }
    }
}
