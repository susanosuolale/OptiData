using System.Threading.Tasks;

namespace OptiData.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentAsync(decimal amount);
    }
}
