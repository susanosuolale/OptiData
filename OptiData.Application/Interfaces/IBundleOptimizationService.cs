using System.Threading.Tasks;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;

namespace OptiData.Application.Interfaces
{
    public interface IBundleOptimizationService
    {
        Task<List<DataBundle>> CalculateOptimalBundleAsync(decimal predictedDataNeedMB, DataProvider provider);
    }
}
