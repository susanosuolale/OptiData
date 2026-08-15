using System;
using MediatR;
using OptiData.Domain.Enums;

namespace OptiData.Application.Bundles.Commands.SchedulePurchase
{
    public class SchedulePurchaseCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public decimal PredictedNeedMB { get; set; }
        public DataProvider Provider { get; set; }
        public int HoursAhead { get; set; }
    }
}
