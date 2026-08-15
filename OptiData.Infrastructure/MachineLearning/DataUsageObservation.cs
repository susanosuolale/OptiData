namespace OptiData.Infrastructure.MachineLearning
{
    // data to teach the model how much a user consumes overtime
    public class DataUsageObservation
    {
        public float HistoricalHours { get; set; }
        public float ConsumedMegabytes { get; set; }
    }
}
