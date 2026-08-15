using System.Threading.Tasks;

namespace OptiData.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendPurchaseNotificationAsync(string message);
    }
}
