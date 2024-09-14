using SalesTaxes.Common.Enums;

namespace SalesTaxes.Common.DTOs;

public class ProductDTO
{
    public Guid Id { get; }
    public string Name { get; }
    public decimal Price { get; }
    public bool IsImported { get; }
    public ProductCategory Category { get; }

    public ProductDTO(string name, decimal price, bool isImported, ProductCategory category)
    {
        Id = Guid.NewGuid();
        Name = name;
        Price = price;
        IsImported = isImported;
        Category = category;
    }

    public bool IsTaxExempt()
    {
        return Category == ProductCategory.Book
            || Category == ProductCategory.Food
            || Category == ProductCategory.Medical;
    }
}
