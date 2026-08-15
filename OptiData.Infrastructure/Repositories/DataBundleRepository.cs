using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OptiData.Application.Interfaces;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;
using OptiData.Infrastructure.Data;

namespace OptiData.Infrastructure.Repositories
{
    public class DataBundleRepository : IDataBundleRepository
    {
        private readonly AppDbContext _context;

        public DataBundleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<DataBundle>> GetAvailableBundlesAsync(DataProvider provider)
        {
            return await _context.DataBundles
                .Where(b => b.Provider == provider)
                .ToListAsync();
        }
    }
}
