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
    public void GenerateReceipt_ShouldReturnCorrectSalesTaxAndTotal_TestCart1()
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
    public void GenerateReceipt_ShouldReturnCorrectSalesTaxAndTotal_TestCart2()
    {
        // Arrange
        var product1 = new ProductDTO("Imported box of chocolates", 10.00m, true, ProductCategory.Food);
        var product2 = new ProductDTO("Imported bottle of perfume", 47.50m, true, ProductCategory.General);

        _cartService.AddCartItem(new CartItemDTO(product1, 1));
        _cartService.AddCartItem(new CartItemDTO(product2, 1));

        // Mocking tax calculations with accurate values
        _mockSalesTaxService.Setup(s => s.CalculateSalesTax(product1)).Returns(0.50m); // Import duty for chocolates
        _mockSalesTaxService.Setup(s => s.CalculateSalesTax(product2)).Returns(7.15m); // Sales tax + import duty for perfume

        // Act
        var receipt = _cartService.GenerateReceipt();

        // Assert
        var lineItem1 = receipt.LineItems.First(item => item.Product.Name == "Imported box of chocolates");
        var lineItem2 = receipt.LineItems.First(item => item.Product.Name == "Imported bottle of perfume");

        // Check individual line items and totals
        Assert.Equal(10.50m, lineItem1.Total);
        Assert.Equal(54.65m, lineItem2.Total);
        Assert.Equal(7.65m, receipt.SalesTaxTotal);
        Assert.Equal(65.15m, receipt.Total);
    }

    [Fact]
    public void GenerateReceipt_ShouldReturnCorrectSalesTaxAndTotal_TestCart3()
    {
        // Arrange
        var product1 = new ProductDTO("Imported bottle of perfume", 27.99m, true, ProductCategory.General);
        var product2 = new ProductDTO("Bottle of perfume", 18.99m, false, ProductCategory.General);
        var product3 = new ProductDTO("Packet of headache pills", 9.75m, false, ProductCategory.Medical);
        var product4 = new ProductDTO("Imported box of chocolates", 11.25m, true, ProductCategory.Food);

        _cartService.AddCartItem(new CartItemDTO(product1, 1));
        _cartService.AddCartItem(new CartItemDTO(product2, 1));
        _cartService.AddCartItem(new CartItemDTO(product3, 1));
        _cartService.AddCartItem(new CartItemDTO(product4, 1));

        // Mocking tax calculations with accurate values
        _mockSalesTaxService.Setup(s => s.CalculateSalesTax(product1)).Returns(4.20m); // Sales tax + import duty for imported perfume
        _mockSalesTaxService.Setup(s => s.CalculateSalesTax(product2)).Returns(1.90m); // Sales tax for regular perfume
        _mockSalesTaxService.Setup(s => s.CalculateSalesTax(product3)).Returns(0m); // No tax for medical product
        _mockSalesTaxService.Setup(s => s.CalculateSalesTax(product4)).Returns(0.60m); // Import duty for imported chocolates

        // Act
        var receipt = _cartService.GenerateReceipt();

        // Assert
        var lineItem1 = receipt.LineItems.First(item => item.Product.Name == "Imported bottle of perfume");
        var lineItem2 = receipt.LineItems.First(item => item.Product.Name == "Bottle of perfume");
        var lineItem3 = receipt.LineItems.First(item => item.Product.Name == "Packet of headache pills");
        var lineItem4 = receipt.LineItems.First(item => item.Product.Name == "Imported box of chocolates");

        // Check individual line items and totals
        Assert.Equal(32.19m, lineItem1.Total);
        Assert.Equal(20.89m, lineItem2.Total);
        Assert.Equal(9.75m, lineItem3.Total);
        Assert.Equal(11.85m, lineItem4.Total);
        Assert.Equal(6.70m, receipt.SalesTaxTotal);
        Assert.Equal(74.68m, receipt.Total);
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
