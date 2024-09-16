using SalesTaxes.Common.DTOs;
using SalesTaxes.Common.Enums;
using SalesTaxes.Services;

namespace SalesTaxes.Tests;

public class InventoryServiceTests
{
    private readonly InventoryService _inventoryService;

    public InventoryServiceTests()
    {
        _inventoryService = new InventoryService();
    }

    [Fact]
    public void CreateInventory_ShouldAddNewItem()
    {
        // Arrange
        var newProduct = new ProductDTO("new book", 15.00m, false, ProductCategory.Book);
        var newItem = new InventoryItemDTO(newProduct, 10);

        // Act
        var addedItem = _inventoryService.CreateInventory(newItem);

        // Assert
        Assert.NotNull(addedItem);
        Assert.Equal(newProduct.Name, addedItem.Product.Name);
        Assert.Equal(10, addedItem.Quantity);
        Assert.Contains(_inventoryService.ListInventory(), i => i.Product.Name == newProduct.Name);
    }

    [Fact]
    public void CreateInventory_ShouldThrowExceptionForDuplicateItem()
    {
        // Arrange
        var existingProduct = new ProductDTO("book", 12.49m, false, ProductCategory.Book);

        // Act
        var duplicateItem = new InventoryItemDTO(existingProduct, 5);

        // Assert
        Assert.Throws<InvalidOperationException>(() => _inventoryService.CreateInventory(duplicateItem));
    }

    [Fact]
    public void RemoveInventoryItem_ShouldRemoveItem()
    {
        // Arrange
        var existingItem = _inventoryService.ListInventory().FirstOrDefault(x => x.Product.Name == "chocolate bar") 
            ?? throw new ApplicationException("Product not found.");

        // Act
        _inventoryService.RemoveInventoryItem(existingItem.Id);

        // Assert
        Assert.DoesNotContain(_inventoryService.ListInventory(), i => i.Product.Name == existingItem.Product.Name);
    }

    [Fact]
    public void UpdateInventoryItem_ShouldUpdateItem()
    {
        // Arrange
        var existingItem = _inventoryService.ListInventory().FirstOrDefault(x => x.Product.Name == "box of imported chocolates") 
            ?? throw new ApplicationException("Product not found.");

        var updatedProduct = new ProductDTO("updated box of imported chocolates", 11.25m, true, ProductCategory.Food);
        var updatedItem = new InventoryItemDTO(updatedProduct, 15, existingItem.Id);

        // Act
        var result = _inventoryService.UpdateInventoryItem(updatedItem);

        // Assert
        Assert.Equal(updatedItem.Quantity, result.Quantity);
        Assert.Contains(_inventoryService.ListInventory(), i => i.Product.Name == updatedProduct.Name && i.Quantity == 15);
    }

    [Fact]
    public void GetInventoryTotalCount_ShouldReturnCorrectCount()
    {
        // Act
        var totalCount = _inventoryService.GetInventoryTotalCount();

        // Assert
        Assert.True(totalCount > 0);
    }

    [Fact]
    public void GetInventoryProductCount_ShouldReturnCorrectCount()
    {
        // Act
        var productCount = _inventoryService.GetInventoryProductCount();

        // Assert
        Assert.True(productCount > 0);
    }
}

