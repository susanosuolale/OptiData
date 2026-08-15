using System;
using OptiData.Domain.Enums;

namespace OptiData.Application.Interfaces
{
    public interface IPurchaseSchedulerService
    {
        void SchedulePurchase(Guid userId, decimal predictedMB, DataProvider provider, int hoursToWait);
    }
}
