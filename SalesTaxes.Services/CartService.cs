using SalesTaxes.Common.DTOs;
using SalesTaxes.Contracts.Services;
using System.Reflection.Metadata.Ecma335;

namespace SalesTaxes.Services;

public class CartService : ICartService
{
    private readonly ISalesTaxService _salesTaxService;
    private readonly List<CartItemDTO> _cartItems;

    public CartService(ISalesTaxService salesTaxService)
    {
        _salesTaxService = salesTaxService;
        _cartItems = [];
    }

    public CartItemDTO AddCartItem(CartItemDTO item)
    {
        _cartItems.Add(item);
        return item;
    }

    public void ClearCartItems()
    {
        _cartItems.Clear();
    }

    public ReceiptDTO GenerateReceipt()
    {
        var receipt = new ReceiptDTO();
        foreach (var item in _cartItems)
        {
            var salesTaxPerItem = _salesTaxService.CalculateSalesTax(item.Product);
            receipt.LineItems.Add(new ReceiptLineItemDTO(item.Product, item.Quantity, salesTaxPerItem));
        }
        return receipt;
    }

    public int GetCartItemsCount()
    {
        return _cartItems.Sum(item => item.Quantity);
    }

    public IList<CartItemDTO> ListCartItems() 
    {
        return _cartItems;
    }

    public void RemoveCartItem(Guid id)
    {
        var item = _cartItems.FirstOrDefault(x => x.Id == id) ?? throw new ApplicationException("No cart item found with the given id.");
        _cartItems.Remove(item);
    }

    public CartItemDTO UpdateCartItemQuantity(Guid id, int quantity)
    {
        var item = _cartItems.FirstOrDefault(x => x.Id == id) ?? throw new ApplicationException("No cart item found with the given id.");
        var updatedItem = new CartItemDTO(item.Product, quantity);
        _cartItems.Remove(item);
        _cartItems.Add(updatedItem);
        return updatedItem;
    }
}
