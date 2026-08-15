using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OptiData.Application.Interfaces;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;

namespace OptiData.Infrastructure.Services
{
    public class GloTelecomProvider : ITelecomProviderService
    {
        public async Task<List<DataBundle>> FetchLatestBundlesFromNetworkAsync()
        {
            // In a live production environment, this code uses an HTTP Client
            // to connect to the actual GLO telecom servers over the internet.
            
            // For this portfolio, we simulate the downloaded data from the GLO servers
            var liveGloBundles = new List<DataBundle>
            {
                new DataBundle { Id = Guid.NewGuid(), Name = "Glo Daily 1GB", DataAmountMB = 1000, Price = 300.00m, Provider = DataProvider.GLO, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Glo 2-Day 2GB", DataAmountMB = 2000, Price = 500.00m, Provider = DataProvider.GLO, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Glo Weekly 7GB", DataAmountMB = 7000, Price = 1500.00m, Provider = DataProvider.GLO, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Glo Monthly 18GB", DataAmountMB = 18000, Price = 4000.00m, Provider = DataProvider.GLO, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Glo Monthly 29GB", DataAmountMB = 29000, Price = 5000.00m, Provider = DataProvider.GLO, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "Glo Monthly 50GB", DataAmountMB = 50000, Price = 10000.00m, Provider = DataProvider.GLO, IsActive = true }
            };

            return await Task.FromResult(liveGloBundles);
        }
    }
}
