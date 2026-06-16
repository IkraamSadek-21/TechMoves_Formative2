namespace TechMoves_API.Strategies
{
    public class DiscountPricingStrategy : IPricingStrategy
    {
        public decimal CalculateCost(decimal baseCost) => baseCost * 0.9m;
    }
}
