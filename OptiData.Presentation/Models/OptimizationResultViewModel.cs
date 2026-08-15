using OptiData.Domain.Entities;

namespace OptiData.Presentation.Models
{
    public class OptimizationResultViewModel
    {
        public List<DataBundle> OptimalBundles { get; set; } = new List<DataBundle>();
        public decimal PredictedTotalMB { get; set; }
        public string ErrorMessage { get; set; }
    }
}
