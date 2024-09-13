using SalesTaxes.Common.DTOs;
using SalesTaxes.Common.Enums;
using SalesTaxes.Contracts.Services;

namespace SalesTaxes.Services;

public class InventoryService : IInventoryService
{
    private readonly List<ProductDTO> _inventory;

    public InventoryService()
    {
        _inventory = [];
        _inventory.Add(new ProductDTO("book", 12.49m, false, ProductCategory.Book));
        _inventory.Add(new ProductDTO("music CD", 14.99m, false, ProductCategory.General));
        _inventory.Add(new ProductDTO("chocolate bar", 0.85m, false, ProductCategory.Food));
        _inventory.Add(new ProductDTO("imported box of chocolates", 10.00m, true, ProductCategory.Food));
        _inventory.Add(new ProductDTO("imported bottle of perfume", 47.50m, true, ProductCategory.General));
        _inventory.Add(new ProductDTO("bottle of perfume", 18.99m, false, ProductCategory.General));
        _inventory.Add(new ProductDTO("packet of headache pills", 9.75m, false, ProductCategory.Medical));
        _inventory.Add(new ProductDTO("box of imported chocolates", 11.25m, true, ProductCategory.Food));
    }

    public ProductDTO CreateInventory(ProductDTO product)
    {
        _inventory.Add(product);
        return product;
    }

    public IList<ProductDTO> ListInventory()
    {
        return _inventory;
    }
}
