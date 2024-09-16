namespace SalesTaxes.Common.DTOs;

public class ReceiptDTO
{
    public IList<ReceiptLineItemDTO> LineItems { get; set; }
    public decimal Subtotal => LineItems.Sum(x => x.Subtotal);
    public decimal SalesTaxTotal => LineItems.Sum(x => x.SalesTaxTotal);
    public decimal Total => LineItems.Sum(x => x.Total);

    public ReceiptDTO()
    {
        LineItems = [];
    }
}
