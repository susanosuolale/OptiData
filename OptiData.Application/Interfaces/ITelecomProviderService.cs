using System.Collections.Generic;
using System.Threading.Tasks;
using OptiData.Domain.Entities;

namespace OptiData.Application.Interfaces
{
    public interface ITelecomProviderService
    {
        // This command goes directly to the MTN or Airtel servers to download their newest bundle prices
        Task<List<DataBundle>> FetchLatestBundlesFromNetworkAsync();
    }
}
