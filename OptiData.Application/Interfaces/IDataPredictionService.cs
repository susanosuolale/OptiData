using System;
using System.Threading.Tasks;

namespace OptiData.Application.Interfaces
{
    public interface IDataPredictionService
    {
        Task<decimal> PredictDataNeedAsync(Guid userId, int hoursAhead);
    }
}
