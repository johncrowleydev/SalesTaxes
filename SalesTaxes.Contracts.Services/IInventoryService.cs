using SalesTaxes.Common.DTOs;

namespace SalesTaxes.Contracts.Services;

public interface IInventoryService
{
    IList<ProductDTO> ListInventory();
    ProductDTO CreateInventory(ProductDTO product);
}
