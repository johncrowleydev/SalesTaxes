namespace SalesTaxes.Common.DTOs;

public class ReceiptLineItemDTO
{
    public ProductDTO Product { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => Product.Price * Quantity;
    public decimal SalesTaxPerItem { get; set; }
    public decimal SalesTaxTotal => SalesTaxPerItem * Quantity;
    public decimal Total => Subtotal + SalesTaxTotal;

    public ReceiptLineItemDTO(ProductDTO product, int quantity, decimal salesTaxPerItem)
    {
        Product = product;
        Quantity = quantity;
        SalesTaxPerItem = salesTaxPerItem;
    }
}
