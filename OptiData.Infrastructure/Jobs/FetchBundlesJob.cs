using System.Threading.Tasks;
using OptiData.Application.Interfaces;
using OptiData.Infrastructure.Data;

// Job that runs when the code starts and fetches data from all providers
// and saves into database
namespace OptiData.Infrastructure.Jobs
{
    public class FetchBundlesJob
    {
        // IEnumerable<ITelecomProviderService> tells the code to get ALL services that implement the ITelecomProviderService interface
        // create them and inject them into this class as a list
        private readonly IEnumerable<ITelecomProviderService> _telecomServices;
        private readonly AppDbContext _context;

        public FetchBundlesJob(IEnumerable<ITelecomProviderService> telecomServices, AppDbContext context)
        {
            _telecomServices = telecomServices;
            _context = context;
        }

        public async Task ExecuteAsync()
        {
            // 0. Clear out old bundles so we don't get duplicates when the app restarts
            _context.DataBundles.RemoveRange(_context.DataBundles);
            await _context.SaveChangesAsync();

            // 1. Loop through EVERY registered telecom provider (MTN, Airtel, etc.)
            foreach (var telecomService in _telecomServices)
            {
                // 2. Download their specific live bundles
                var liveBundles = await telecomService.FetchLatestBundlesFromNetworkAsync();

                // 3. Save them into our physical SQL database
                _context.DataBundles.AddRange(liveBundles);
            }
            
            await _context.SaveChangesAsync();
        }
    }
}
