namespace TechMoves_Formative2.Strategies
{
    public class DiscountPricingStrategy : IPricingStrategy
    {
        public decimal CalculateCost(decimal baseCost)
        {
            return baseCost * 0.9m;
        }
    }
}