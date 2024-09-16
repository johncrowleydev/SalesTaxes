using SalesTaxes.Common.DTOs;
using SalesTaxes.Contracts.Services;

namespace SalesTaxes.Services;

public class SalesTaxService : ISalesTaxService
{
    const decimal BASIC_SALES_TAX_RATE = 0.10m;
    const decimal IMPORT_DUTY_RATE = 0.05m;
    const decimal TAX_ROUNDING = 0.05m;

    private readonly ISessionService _sessionService;

    public SalesTaxService(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public decimal CalculateSalesTax(ProductDTO product)
    {
        var currentUser = _sessionService.GetCurrentUser();
        var isTaxFreeState = currentUser.State.StartsWith("m", StringComparison.OrdinalIgnoreCase);
        if (isTaxFreeState) return 0;

        decimal taxRate = 0;
        if (!product.IsTaxExempt()) taxRate += BASIC_SALES_TAX_RATE;
        if (product.IsImported) taxRate += IMPORT_DUTY_RATE;

        decimal salesTax = product.Price * taxRate;
        return RoundUpToNearestFiveCents(salesTax);
    }

    public static decimal RoundUpToNearestFiveCents(decimal value)
    {
        return Math.Ceiling(value / TAX_ROUNDING) * TAX_ROUNDING;
    }
}
