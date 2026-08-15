using Microsoft.ML.Data;

namespace OptiData.Infrastructure.MachineLearning
{
    // what the model outputs after prediction
    public class DataUsagePrediction
    {
        // "Score" is the default name ML.NET gives to the output of a regression model.
        // so whatever the predictionEngine calculates, it assigns it to the "Score"
        // this extracts the data from score and puts it in PredictedMegabytes
        [ColumnName("Score")]
        public float PredictedMegabytes { get; set; }
    }
}
