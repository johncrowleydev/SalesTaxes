using SalesTaxes.Common.DTOs;
using SalesTaxes.Common.Enums;
using SalesTaxes.Contracts.Services;

namespace SalesTaxes.Services;

public class InventoryService : IInventoryService
{
    private readonly List<InventoryItemDTO> _inventory;

    public InventoryService()
    {
        var random = new Random();
        _inventory =
        [
            new InventoryItemDTO(new ProductDTO("book", 12.49m, false, ProductCategory.Book), random.Next(1, 100)),
            new InventoryItemDTO(new ProductDTO("music CD", 14.99m, false, ProductCategory.General), random.Next(1, 100)),
            new InventoryItemDTO(new ProductDTO("chocolate bar", 0.85m, false, ProductCategory.Food), random.Next(1, 100)),
            new InventoryItemDTO(new ProductDTO("imported box of chocolates", 10.00m, true, ProductCategory.Food), random.Next(1, 100)),
            new InventoryItemDTO(new ProductDTO("imported bottle of perfume", 47.50m, true, ProductCategory.General), random.Next(1, 100)),
            new InventoryItemDTO(new ProductDTO("bottle of perfume", 18.99m, false, ProductCategory.General), random.Next(1, 100)),
            new InventoryItemDTO(new ProductDTO("packet of headache pills", 9.75m, false, ProductCategory.Medical), random.Next(1, 100)),
            new InventoryItemDTO(new ProductDTO("box of imported chocolates", 11.25m, true, ProductCategory.Food), random.Next(1, 100))
        ];
    }

    public InventoryItemDTO CreateInventory(InventoryItemDTO item)
    {
        var duplicateItem = _inventory.Any(existingItem =>
            existingItem.Id == item.Id ||
            existingItem.Product.Id == item.Product.Id ||
            existingItem.Product.Name.Equals(item.Product.Name, StringComparison.OrdinalIgnoreCase)
        );

        if (duplicateItem)
        {
            throw new InvalidOperationException("An inventory item for this product already exists.");
        }

        _inventory.Add(item);
        return item;
    }

    public int GetInventoryTotalCount()
    {
        return _inventory.Sum(x => x.Quantity);
    }

    public int GetInventoryProductCount()
    {
        return _inventory.Count;
    }

    public IList<InventoryItemDTO> ListInventory()
    {
        return _inventory;
    }

    public void RemoveInventoryItem(Guid id)
    {
        var itemToRemove = _inventory.SingleOrDefault(x => x.Id == id);
        if (itemToRemove != null)
        {
            _inventory.Remove(itemToRemove);
        }
        else
        {
            throw new ArgumentException("Item not found", nameof(id));
        }
    }

    public InventoryItemDTO UpdateInventoryItem(InventoryItemDTO updatedItem)
    {
        var existingItem = _inventory.SingleOrDefault(x => x.Id == updatedItem.Id);
        if (existingItem == null)
        {
            throw new ArgumentException("Item not found", nameof(updatedItem.Id));
        }
        _inventory[_inventory.IndexOf(existingItem)] = updatedItem;
        return updatedItem;
    }
}
