using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OptiData.Domain.Entities;
using OptiData.Application.Interfaces;

namespace OptiData.Application.Bundles.Commands.OptimizeBundles
{
    public class OptimizeBundlesCommandHandler : IRequestHandler<OptimizeBundlesCommand, OptimizeBundlesResult>
    {
        private readonly IDataPredictionService _predictionService;
        private readonly IBundleOptimizationService _optimizationService;
        private readonly IPurchaseSchedulerService _purchaseScheduler;

        public OptimizeBundlesCommandHandler(
            IDataPredictionService predictionService,
            IBundleOptimizationService optimizationService,
            IPurchaseSchedulerService purchaseScheduler)
        {
            _predictionService = predictionService;
            _optimizationService = optimizationService;
            _purchaseScheduler = purchaseScheduler;
        }

        public async Task<OptimizeBundlesResult> Handle(OptimizeBundlesCommand request, CancellationToken cancellationToken)
        {
            // predicts exact mb needed
            var predictedNeedMB = await _predictionService.PredictDataNeedAsync(request.UserId, request.HoursAhead);
            // takes predicted exam mb needed and passes it to optimization service to find the absolute cheapest bundle
            var optimalBundles = await _optimizationService.CalculateOptimalBundleAsync(predictedNeedMB, request.Provider);

            return new OptimizeBundlesResult 
            { 
                Bundles = optimalBundles, 
                PredictedTotalMB = predictedNeedMB 
            };
        }
    }
}
