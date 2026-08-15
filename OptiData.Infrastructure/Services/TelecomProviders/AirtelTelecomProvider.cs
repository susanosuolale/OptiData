using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OptiData.Application.Interfaces;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;

namespace OptiData.Infrastructure.Services
{
    public class AirtelTelecomProvider : ITelecomProviderService
    {
        public async Task<List<DataBundle>> FetchLatestBundlesFromNetworkAsync()
        {
            // For this portfolio, we simulate the downloaded data from the Airtel servers.
            // We are using a simulation because we do not have the private API keys required 
            // to access the actual Airtel telecommunication servers or user's private data.
            var liveAirtelBundles = new List<DataBundle>
            {
                new DataBundle { Id = Guid.NewGuid(), Name = "Airtel Daily 1GB", DataAmountMB = 1000, Price = 300.00m, Provider = DataProvider.Airtel, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Airtel 2-Day 2GB", DataAmountMB = 2000, Price = 500.00m, Provider = DataProvider.Airtel, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Airtel Weekly 6GB", DataAmountMB = 6000, Price = 1500.00m, Provider = DataProvider.Airtel, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Airtel Monthly 1.2GB", DataAmountMB = 1200, Price = 1000.00m, Provider = DataProvider.Airtel, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Airtel Monthly 20GB", DataAmountMB = 20000, Price = 5000.00m, Provider = DataProvider.Airtel, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Airtel Monthly 40GB", DataAmountMB = 40000, Price = 10000.00m, Provider = DataProvider.Airtel, IsActive = true }
            };

            return await Task.FromResult(liveAirtelBundles);
        }
    }
}
