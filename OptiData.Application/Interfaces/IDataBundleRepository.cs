using System.Collections.Generic;
using System.Threading.Tasks;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;

namespace OptiData.Application.Interfaces
{
    public interface IDataBundleRepository
    {
        Task<List<DataBundle>> GetAvailableBundlesAsync(DataProvider provider);
    }
}
