namespace SalesTaxes.Common.DTOs;

public class CartItemDTO
{
    public Guid Id { get; }
    public ProductDTO Product { get; }
    public int Quantity { get; }

    public CartItemDTO(ProductDTO product, int quantity)
    {
        Id = Guid.NewGuid();
        Product = product;
        Quantity = quantity;
    }
}
