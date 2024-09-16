using SalesTaxes.Common.DTOs;

namespace SalesTaxes.Contracts.Services;

public interface ISalesTaxService
{
    decimal CalculateSalesTax(ProductDTO product);
}
