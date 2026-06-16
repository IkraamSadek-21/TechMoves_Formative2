namespace TechMoves_API.Services
{
    public interface ICurrencyConversionService
    {
        Task<decimal> ConvertUsdToZarAsync(decimal usdAmount);
    }
}
