using SalesTaxes.Common.DTOs;

namespace SalesTaxes.Contracts.Services;

public interface IInventoryService
{
    InventoryItemDTO CreateInventory(InventoryItemDTO product);
    int GetInventoryTotalCount();
    int GetInventoryProductCount();
    IList<InventoryItemDTO> ListInventory();
    void RemoveInventoryItem(Guid id);
    InventoryItemDTO UpdateInventoryItem(InventoryItemDTO updatedItem);
}
