using System;
using OptiData.Domain.Enums;

// data structure to hold the data bundles downloaded from
// the web provider
namespace OptiData.Domain.Entities
{
    public class DataBundle
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal DataAmountMB { get; set; }
        public decimal Price { get; set; }
        public int ValidityDurationHours { get; set; }
        public bool IsActive { get; set; }
        public DataProvider Provider { get; set; }
    }
}
