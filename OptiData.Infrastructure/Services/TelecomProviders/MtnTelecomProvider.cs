using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OptiData.Application.Interfaces;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;

namespace OptiData.Infrastructure.Services
{
    public class MtnTelecomProvider : ITelecomProviderService
    {
        public async Task<List<DataBundle>> FetchLatestBundlesFromNetworkAsync()
        {
            // In a live production environment, this code uses an HTTP Client
            // to connect to the actual MTN telecom servers over the internet.
            
            // For this portfolio, we simulate the downloaded data from the MTN servers
            var liveMtnBundles = new List<DataBundle>
            {
                new DataBundle { Id = Guid.NewGuid(), Name = "MTN Daily 1.5GB", DataAmountMB = 1500, Price = 500.00m, Provider = DataProvider.MTN, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "MTN 2-Day 2GB", DataAmountMB = 2000, Price = 500.00m, Provider = DataProvider.MTN, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "MTN Weekly 6GB", DataAmountMB = 6000, Price = 1500.00m, Provider = DataProvider.MTN, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "MTN Monthly 12GB", DataAmountMB = 12000, Price = 3500.00m, Provider = DataProvider.MTN, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "MTN Monthly 20GB", DataAmountMB = 20000, Price = 5000.00m, Provider = DataProvider.MTN, IsActive = true },
                new DataBundle { Id = Guid.NewGuid(), Name = "MTN Monthly 40GB", DataAmountMB = 40000, Price = 10000.00m, Provider = DataProvider.MTN, IsActive = true }
            };

            return await Task.FromResult(liveMtnBundles);
        }
    }
}
