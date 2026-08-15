using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OptiData.Application.Interfaces;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;

namespace OptiData.Infrastructure.Services
{
    public class NineMobileTelecomProvider : ITelecomProviderService
    {
        public async Task<List<DataBundle>> FetchLatestBundlesFromNetworkAsync()
        {
            // In a live production environment, this code uses an HTTP Client
            // to connect to the actual 9Mobile telecom servers over the internet.
            
            // For this portfolio, we simulate the downloaded data from the 9Mobile servers
            var liveNineMobileBundles = new List<DataBundle>
            {
                new DataBundle { Id = Guid.NewGuid(), Name = "9Mobile Daily 1GB", DataAmountMB = 1000, Price = 300.00m, Provider = DataProvider.NineMobile, IsActive = true, ValidityDurationHours = 24 },
                new DataBundle { Id = Guid.NewGuid(), Name = "9Mobile 3-Day 2GB", DataAmountMB = 2000, Price = 500.00m, Provider = DataProvider.NineMobile, IsActive = true, ValidityDurationHours = 72 },
                new DataBundle { Id = Guid.NewGuid(), Name = "9Mobile Weekly 7GB", DataAmountMB = 7000, Price = 1500.00m, Provider = DataProvider.NineMobile, IsActive = true, ValidityDurationHours = 168 },
                new DataBundle { Id = Guid.NewGuid(), Name = "9Mobile Monthly 15GB", DataAmountMB = 15000, Price = 4000.00m, Provider = DataProvider.NineMobile, IsActive = true, ValidityDurationHours = 720 },
                new DataBundle { Id = Guid.NewGuid(), Name = "9Mobile Monthly 24GB", DataAmountMB = 24000, Price = 5000.00m, Provider = DataProvider.NineMobile, IsActive = true, ValidityDurationHours = 720 },
                new DataBundle { Id = Guid.NewGuid(), Name = "9Mobile Monthly 40GB", DataAmountMB = 40000, Price = 10000.00m, Provider = DataProvider.NineMobile, IsActive = true, ValidityDurationHours = 720 }
            };

            return await Task.FromResult(liveNineMobileBundles);
        }
    }
}
