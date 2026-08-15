using System;

namespace OptiData.Domain.Entities
{
    // entity to store user data usage
    // including the timestamp the usage was recorded and how much data was used
    public class UsageRecord
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal DataConsumedMB { get; set; }
    }
}
