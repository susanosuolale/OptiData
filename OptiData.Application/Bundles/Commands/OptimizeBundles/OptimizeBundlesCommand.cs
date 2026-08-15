using System;
using MediatR;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;

namespace OptiData.Application.Bundles.Commands.OptimizeBundles
{
    public class OptimizeBundlesResult
    {
        public List<DataBundle> Bundles { get; set; } = new List<DataBundle>();
        public decimal PredictedTotalMB { get; set; }
    }

    // returns an OptimizeBundlesResult
    public class OptimizeBundlesCommand : IRequest<OptimizeBundlesResult>
    {
        public Guid UserId { get; set; }
        // represents the exact timeframe the user wants the data to last for
        public int HoursAhead { get; set; }
        public DataProvider Provider { get; set; }
    }
}
