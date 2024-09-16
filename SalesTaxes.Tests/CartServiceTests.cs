using Moq;
using SalesTaxes.Common.DTOs;
using SalesTaxes.Common.Enums;
using SalesTaxes.Contracts.Services;
using SalesTaxes.Services;

namespace SalesTaxes.Tests;

public class CartServiceTests
{
    private readonly Mock<ISalesTaxService> _mockSalesTaxService;
    private readonly CartService _cartService;

    public CartServiceTests()
    {
        _mockSalesTaxService = new Mock<ISalesTaxService>();
        _cartService = new CartService(_mockSalesTaxService.Object);
    }

    [Fact]
    public void AddCartItem_ShouldAddItemToCart()
    {
        // Arrange
        var product = new ProductDTO("Book", 12.49m, false, ProductCategory.Book);
        var cartItem = new CartItemDTO(product, 1);

        // Act
        var result = _cartService.AddCartItem(cartItem);

        // Assert
        Assert.Contains(cartItem, _cartService.ListCartItems());
        Assert.Equal(cartItem, result);
    }

    [Fact]
    public void ClearCartItems_ShouldRemoveAllItemsFromCart()
    {
        // Arrange
        var product = new ProductDTO("Book", 12.49m, false, ProductCategory.Book);
        _cartService.AddCartItem(new CartItemDTO(product, 1));

        // Act
        _cartService.ClearCartItems();

        // Assert
        Assert.Empty(_cartService.ListCartItems());
    }

    [Fact]
    public void GenerateReceipt_ShouldReturnCorrectSalesTaxAndTotal()
    {
        // Arrange
        var product1 = new ProductDTO("Book", 12.49m, false, ProductCategory.Book);
        var product2 = new ProductDTO("Music CD", 14.99m, false, ProductCategory.General);
        var product3 = new ProductDTO("Chocolate bar", 0.85m, false, ProductCategory.Food);

        _cartService.AddCartItem(new CartItemDTO(product1, 1));
        _cartService.AddCartItem(new CartItemDTO(product2, 1));
        _cartService.AddCartItem(new CartItemDTO(product3, 1));

        // Mocking tax calculations with accurate values
        _mockSalesTaxService.Setup(s => s.CalculateSalesTax(product1)).Returns(0m); // No tax for Book
        _mockSalesTaxService.Setup(s => s.CalculateSalesTax(product2)).Returns(1.50m); // Tax for Music CD
        _mockSalesTaxService.Setup(s => s.CalculateSalesTax(product3)).Returns(0m); // No tax for food

        // Act
        var receipt = _cartService.GenerateReceipt();

        // Assert
        var lineItem1 = receipt.LineItems.First(item => item.Product.Name == "Book");
        var lineItem2 = receipt.LineItems.First(item => item.Product.Name == "Music CD");
        var lineItem3 = receipt.LineItems.First(item => item.Product.Name == "Chocolate bar");

        // Check individual line items and totals
        Assert.Equal(12.49m, lineItem1.Total);
        Assert.Equal(16.49m, lineItem2.Total);
        Assert.Equal(0.85m, lineItem3.Total);
        Assert.Equal(1.50m, receipt.SalesTaxTotal);
        Assert.Equal(29.83m, receipt.Total);
    }

    [Fact]
    public void GetCartItemsCount_ShouldReturnCorrectCount()
    {
        // Arrange
        var product = new ProductDTO("Book", 12.49m, false, ProductCategory.Book);
        _cartService.AddCartItem(new CartItemDTO(product, 2));

        // Act
        var count = _cartService.GetCartItemsCount();

        // Assert
        Assert.Equal(2, count);
    }

    [Fact]
    public void RemoveCartItem_ShouldRemoveSpecificItem()
    {
        // Arrange
        var product = new ProductDTO("Book", 12.49m, false, ProductCategory.Book);
        var cartItem = new CartItemDTO(product, 1);
        _cartService.AddCartItem(cartItem);

        // Act
        _cartService.RemoveCartItem(cartItem.Id);

        // Assert
        Assert.DoesNotContain(cartItem, _cartService.ListCartItems());
    }

    [Fact]
    public void UpdateCartItemQuantity_ShouldUpdateQuantity()
    {
        // Arrange
        var product = new ProductDTO("Book", 12.49m, false, ProductCategory.Book);
        var cartItem = new CartItemDTO(product, 1);
        _cartService.AddCartItem(cartItem);

        // Act
        var updatedItem = _cartService.UpdateCartItemQuantity(cartItem.Id, 3);

        // Assert
        Assert.Equal(3, updatedItem.Quantity);
        Assert.Contains(updatedItem, _cartService.ListCartItems());
    }
}
