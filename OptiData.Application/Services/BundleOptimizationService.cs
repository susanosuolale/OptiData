using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OptiData.Application.Interfaces;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;

namespace OptiData.Application.Services
{
    public class BundleOptimizationService : IBundleOptimizationService
    {
        private readonly IDataBundleRepository _repository;

        public BundleOptimizationService(IDataBundleRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<DataBundle>> CalculateOptimalBundleAsync(decimal predictedDataNeedMB, DataProvider provider)
        {
            var availableBundles = await _repository.GetAvailableBundlesAsync(provider);
            var combinations = new List<DataBundle>();

            if (!availableBundles.Any() || predictedDataNeedMB <= 0)
                return combinations;

            // Sort bundles from largest to smallest by Data Amount
            var sortedBundles = availableBundles.OrderByDescending(b => b.DataAmountMB).ToList();
            var remainingMB = predictedDataNeedMB;

            // Greedy combination algorithm
            foreach (var bundle in sortedBundles)
            {
                while (remainingMB >= bundle.DataAmountMB)
                {
                    combinations.Add(bundle);
                    remainingMB -= bundle.DataAmountMB;
                }
            }

            // If there's still a tiny remainder (e.g., 50MB) that couldn't be perfectly filled
            // by dividing into whole bundles, we just add the smallest possible bundle to cover it
            if (remainingMB > 0)
            {
                var smallestBundle = sortedBundles.LastOrDefault(); // It's sorted descending, so Last is smallest
                if (smallestBundle != null)
                {
                    combinations.Add(smallestBundle);
                }
            }

            return combinations;
        }
    }
}
