namespace TechMoves_Formative2.Strategies
{
    public interface IPricingStrategy
    {
        decimal CalculateCost(decimal baseCost);
    }
}