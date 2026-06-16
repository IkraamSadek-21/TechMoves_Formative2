namespace TechMoves_API.Strategies
{
    public class StandardPricingStrategy : IPricingStrategy
    {
        public decimal CalculateCost(decimal baseCost) => baseCost;
    }
}
