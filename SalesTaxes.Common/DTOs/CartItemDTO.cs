namespace SalesTaxes.Common.DTOs;

public class CartItemDTO
{
    public ProductDTO Product { get; }
    public int Quantity { get; }

    public CartItemDTO(ProductDTO product, int quantity)
    {
        Product = product;
        Quantity = quantity;
    }
}
