using Moq;
using SalesTaxes.Common.DTOs;
using SalesTaxes.Common.Enums;
using SalesTaxes.Contracts.Services;
using SalesTaxes.Services;

namespace SalesTaxes.Tests;

public class SalesTaxServiceTests
{
    private readonly SalesTaxService _salesTaxService;
    private readonly Mock<ISessionService> _mockSessionService;

    public SalesTaxServiceTests()
    {
        _mockSessionService = new Mock<ISessionService>();
        _salesTaxService = new SalesTaxService(_mockSessionService.Object);
    }

    [Fact]
    public void CalculateSalesTax_ShouldReturnZero_ForTaxFreeState()
    {
        // Arrange
        var taxFreeUser = new UserDTO("Alice", "Montana");
        _mockSessionService.Setup(s => s.GetCurrentUser()).Returns(taxFreeUser);

        var product = new ProductDTO("book", 12.49m, false, ProductCategory.Book);

        // Act
        var result = _salesTaxService.CalculateSalesTax(product);

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void CalculateSalesTax_ShouldReturnBasicSalesTax_ForNonExemptProduct()
    {
        // Arrange
        var user = new UserDTO("Bob", "California");
        _mockSessionService.Setup(s => s.GetCurrentUser()).Returns(user);

        var product = new ProductDTO("music CD", 14.99m, false, ProductCategory.General);

        // Act
        var result = _salesTaxService.CalculateSalesTax(product);

        // Assert
        Assert.Equal(1.50m, result); // 10% of 14.99 rounded to nearest 0.05
    }

    [Fact]
    public void CalculateSalesTax_ShouldReturnImportDuty_ForImportedProduct()
    {
        // Arrange
        var user = new UserDTO("Charlie", "Texas");
        _mockSessionService.Setup(s => s.GetCurrentUser()).Returns(user);

        var importedProduct = new ProductDTO("imported chocolates", 10.00m, true, ProductCategory.Food);

        // Act
        var result = _salesTaxService.CalculateSalesTax(importedProduct);

        // Assert
        Assert.Equal(0.50m, result); // 5% of 10.00 rounded to nearest 0.05
    }

    [Fact]
    public void CalculateSalesTax_ShouldReturnCombinedTax_ForNonExemptAndImportedProduct()
    {
        // Arrange
        var user = new UserDTO("David", "New York");
        _mockSessionService.Setup(s => s.GetCurrentUser()).Returns(user);

        var importedPerfume = new ProductDTO("imported bottle of perfume", 47.50m, true, ProductCategory.General);

        // Act
        var result = _salesTaxService.CalculateSalesTax(importedPerfume);

        // Assert
        Assert.Equal(7.15m, result); // 15% (10% + 5%) of 47.50 rounded to nearest 0.05
    }
}
