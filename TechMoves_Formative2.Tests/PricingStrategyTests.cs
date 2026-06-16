using TechMoves_Formative2.Strategies;
using Xunit;

namespace TechMoves_Formative2.Tests
{
    public class PricingStrategyTests
    {
        [Fact]
        public void StandardPricingStrategy_ReturnsSameAmount()
        {
            // Arrange
            var strategy = new StandardPricingStrategy();
            decimal baseCost = 1000m;

            // Act
            var result = strategy.CalculateCost(baseCost);

            // Assert
            Assert.Equal(1000m, result);
        }

        [Fact]
        public void DiscountPricingStrategy_AppliesTenPercentDiscount()
        {
            // Arrange
            var strategy = new DiscountPricingStrategy();
            decimal baseCost = 1000m;

            // Act
            var result = strategy.CalculateCost(baseCost);

            // Assert
            Assert.Equal(900m, result);
        }
    }
}