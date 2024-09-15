namespace SalesTaxes.Common.DTOs;

public class InventoryItemDTO
{
    public Guid Id { get; private set; }
    public ProductDTO Product { get; }
    public int Quantity { get; }

    public InventoryItemDTO(ProductDTO product, int quantity, Guid? id = null)
    {
        Id = id ?? Guid.NewGuid();
        Product = product;
        Quantity = quantity;
    }
}
