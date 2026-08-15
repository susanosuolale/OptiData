using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;
using OptiData.Application.Interfaces;
using OptiData.Infrastructure.Data;

namespace OptiData.Infrastructure.MachineLearning
{
    public class DataPredictionService : IDataPredictionService
    {
        private readonly MLContext _mlContext;
        private readonly AppDbContext _dbContext;

        public DataPredictionService(AppDbContext dbContext)
        {
            _mlContext = new MLContext(seed: 0);
            _dbContext = dbContext;
        }

        public async Task<decimal> PredictDataNeedAsync(Guid userId, int hoursAhead)
        {
            // 1. Fetch real historical data from our SQL Database
            var records = await _dbContext.UsageRecords
                .Where(r => r.UserId == userId)
                .OrderBy(r => r.Timestamp)
                .ToListAsync();

            if (!records.Any())
            {
                return 0m; 
            }

            // 2. Properly Reshape Data for Data Science (Duration vs Total MB)
            var historicalData = new List<DataUsageObservation>();
            var newestTime = records.Max(r => r.Timestamp);
            var oldestTime = records.Min(r => r.Timestamp);
            var totalHoursAvailable = (newestTime - oldestTime).TotalHours;

            // We create training samples for different time durations (1 hour up to 720 hours / 1 month)
            // to calculate total mbs used in this different time duration
            // this will serve as training data for our prediction model
            var durationWindows = new List<int> { 1, 2, 6, 12, 24, 48, 72, 168, 336, 720 }; 
            
            foreach(var duration in durationWindows)
            {
                // We only create a training point if the user has been active for that long
                if (duration <= totalHoursAvailable || duration == 1) 
                {
                    // Calculate exactly how much total data was used in that specific duration window
                    // newestTime is exact time right now 
                    // startTime is time right now - duration
                    
                    var startTime = newestTime.AddHours(-duration);
                    var totalMbInDuration = records
                        .Where(r => r.Timestamp >= startTime && r.Timestamp <= newestTime)
                        .Sum(r => r.DataConsumedMB);
                    
                    historicalData.Add(new DataUsageObservation 
                    { 
                        HistoricalHours = duration, 
                        ConsumedMegabytes = (float)totalMbInDuration 
                    });
                }
            }

            var trainingData = _mlContext.Data.LoadFromEnumerable(historicalData);

            // 3. Build the pipeline and train the model
            // BREAKDOWN:
            // Step A: We take our input data (HistoricalHours) and package it into a single column named "Features".
            // Step B: We attach the Ordinary Least Squares (Ols) algorithm to our pipeline.
            // Step C: We tell the algorithm that its goal is to predict the "ConsumedMegabytes" column based on the "Features".
            var pipeline = _mlContext.Transforms.Concatenate("Features", new[] { "HistoricalHours" })
                .Append(_mlContext.Regression.Trainers.LbfgsPoissonRegression(labelColumnName: "ConsumedMegabytes"));
            
            var model = pipeline.Fit(trainingData);

            // 4. Make the prediction
            var predictionEngine = _mlContext.Model.CreatePredictionEngine<DataUsageObservation, DataUsagePrediction>(model);
            
            // Now, because the model is trained on Duration vs TotalMB, we can simply pass the requested duration.
            // If the requested duration is vastly greater than our maximum training window (720 hours / 1 month),
            // ML.NET's Sdca regression can struggle to extrapolate linearly due to feature normalization.
            // We'll accurately predict the baseline for 720 hours and multiply by how many 720 hours are in the requested time frame (like Years).
            int predictionHours = hoursAhead > 720 ? 720 : hoursAhead;

            var input = new DataUsageObservation { HistoricalHours = predictionHours };
            var prediction = predictionEngine.Predict(input);

            var finalPredictedTotal = (decimal)prediction.PredictedMegabytes;

            if (hoursAhead > 720)
            {
                var multiplier = (decimal)hoursAhead / 720m;
                finalPredictedTotal *= multiplier;
            }
            // if prediction is less than 10mb, return 10mb
            if (finalPredictedTotal < 10m) finalPredictedTotal = 10m; // Minimum fallback

            return finalPredictedTotal;
        }
    }
}
