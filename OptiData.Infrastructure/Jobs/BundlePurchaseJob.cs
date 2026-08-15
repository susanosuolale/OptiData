using System;
using System.Threading.Tasks;
using OptiData.Application.Interfaces;
using OptiData.Domain.Enums;
using OptiData.Infrastructure.Data;

// background job that buys bundle for user one hour to the time
// their data will expire
namespace OptiData.Infrastructure.Jobs
{
    public class BundlePurchaseJob
    {
        private readonly IBundleOptimizationService _optimizationService;
        private readonly IPaymentService _paymentService;
        private readonly AppDbContext _context;
        private readonly INotificationService _notificationService;

        public BundlePurchaseJob(
            IBundleOptimizationService optimizationService,
            IPaymentService paymentService,
            AppDbContext context,
            INotificationService notificationService)
        {
            _optimizationService = optimizationService;
            _paymentService = paymentService;
            _context = context;
            _notificationService = notificationService;
        }

        // This method will be called automatically by Hangfire in the background
        public async Task ExecutePurchaseSimulationAsync(Guid userId, decimal predictedMB, DataProvider provider)
        {
            // 1. Enterprise State-Check: Did the user already buy data early?
            var user = await _context.Users.FindAsync(userId);
            if (user != null && user.CurrentBalanceMB > 50)
            {
                Console.WriteLine($"[Hangfire Background Job] User {userId} has {user.CurrentBalanceMB}MB of data. Forcing purchase for demonstration purposes.");
                // return; // Gracefully exit without double-charging them
            }
            // 1. Calculate the absolute best bundle to buy for the user
            var optimalBundles = await _optimizationService.CalculateOptimalBundleAsync(predictedMB, provider);

            if (optimalBundles == null || optimalBundles.Count == 0) return;

            decimal totalPrice = System.Linq.Enumerable.Sum(optimalBundles, b => b.Price);
            decimal totalMB = System.Linq.Enumerable.Sum(optimalBundles, b => b.DataAmountMB);
            string bundleNames = string.Join(" + ", System.Linq.Enumerable.Select(optimalBundles, b => b.Name));

            // 2. Connect to the bank/payment gateway to deduct the money
            var paymentSuccessful = await _paymentService.ProcessPaymentAsync(totalPrice);

            if (paymentSuccessful)
            {
                // 3. Simulate buying the bundle from the telecom provider so the user never loses their internet connection
                user.CurrentBalanceMB += totalMB;
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"[Hangfire Background Job] Successfully simulated purchase of {bundleNames} bundle(s) for User {userId}. New Balance: {user.CurrentBalanceMB}MB.");

                await _notificationService.SendPurchaseNotificationAsync(
                    $"✨ SUCCESS: {bundleNames} Auto-Purchased! Your internet was completely uninterrupted. New Balance: {user.CurrentBalanceMB}MB");
            }
        }
    }
}
