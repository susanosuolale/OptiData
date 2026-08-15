using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using OptiData.Application.Bundles.Commands.OptimizeBundles;
using OptiData.Application.Interfaces;
using OptiData.Domain.Entities;
using OptiData.Domain.Enums;
using Xunit;

namespace OptiData.Tests.Application
{
    public class OptimizeBundlesCommandHandlerTests
    {
        private readonly Mock<IDataPredictionService> _mockPredictionService;
        private readonly Mock<IBundleOptimizationService> _mockOptimizationService;
        private readonly Mock<IPurchaseSchedulerService> _mockPurchaseScheduler;
        private readonly OptimizeBundlesCommandHandler _handler;

        public OptimizeBundlesCommandHandlerTests()
        {
            _mockPredictionService = new Mock<IDataPredictionService>();
            _mockOptimizationService = new Mock<IBundleOptimizationService>();
            _mockPurchaseScheduler = new Mock<IPurchaseSchedulerService>();
            
            _handler = new OptimizeBundlesCommandHandler(
                _mockPredictionService.Object,
                _mockOptimizationService.Object,
                _mockPurchaseScheduler.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnOptimalBundle_WhenPredictionIsMade()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new OptimizeBundlesCommand
            {
                UserId = userId,
                HoursAhead = 720, // 1 month
                Provider = DataProvider.MTN
            };

            // Predict 20GB (20480 MB) need
            _mockPredictionService
                .Setup(s => s.PredictDataNeedAsync(userId, 720))
                .ReturnsAsync(20480m);

            var expectedBundle = new DataBundle 
            { 
                Id = Guid.NewGuid(), 
                Name = "25GB Plan", 
                DataAmountMB = 25600, 
                Price = 6000, 
                Provider = DataProvider.MTN 
            };

            _mockOptimizationService
                .Setup(s => s.CalculateOptimalBundleAsync(20480m, DataProvider.MTN))
                .ReturnsAsync(new System.Collections.Generic.List<DataBundle> { expectedBundle });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Bundles.Should().NotBeEmpty();
            result.Bundles[0].Name.Should().Be("25GB Plan");
            result.Bundles[0].DataAmountMB.Should().Be(25600);
            result.Bundles[0].Price.Should().Be(6000);
        }
    }
}
