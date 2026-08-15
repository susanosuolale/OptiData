using System;
using OptiData.Domain.Enums;

namespace OptiData.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal CurrentBalanceMB { get; set; }
        public DataProvider PreferredProvider { get; set; }
    }
}
