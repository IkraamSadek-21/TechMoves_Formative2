namespace TechMoves_Formative2.Strategies
{
    public class StandardPricingStrategy : IPricingStrategy
    {
        public decimal CalculateCost(decimal baseCost)
        {
            return baseCost;
        }
    }
}