namespace TechMoves_Formative2.Services
{
    public interface ICurrencyConversionService
    {
        Task<decimal> ConvertUsdToZarAsync(decimal usdAmount);
    }
}