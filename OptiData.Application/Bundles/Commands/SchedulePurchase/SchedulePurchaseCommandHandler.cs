using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OptiData.Application.Interfaces;

namespace OptiData.Application.Bundles.Commands.SchedulePurchase
{
    public class SchedulePurchaseCommandHandler : IRequestHandler<SchedulePurchaseCommand, bool>
    {
        private readonly IPurchaseSchedulerService _purchaseScheduler;

        public SchedulePurchaseCommandHandler(IPurchaseSchedulerService purchaseScheduler)
        {
            _purchaseScheduler = purchaseScheduler;
        }

        public Task<bool> Handle(SchedulePurchaseCommand request, CancellationToken cancellationToken)
        {
            // Calculate when to buy the next bundle (1 hour before expiration)
            var hoursToWait = request.HoursAhead > 1 ? request.HoursAhead - 1 : 0;
            
            // Explicitly schedule the job only when this specific handler is manually called by the user
            _purchaseScheduler.SchedulePurchase(request.UserId, request.PredictedNeedMB, request.Provider, hoursToWait);
            
            return Task.FromResult(true);
        }
    }
}
