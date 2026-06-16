namespace TechMoves_API.Strategies
{
    public interface IPricingStrategy
    {
        decimal CalculateCost(decimal baseCost);
    }
}
