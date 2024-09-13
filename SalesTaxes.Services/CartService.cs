using SalesTaxes.Common.DTOs;
using SalesTaxes.Contracts.Services;

namespace SalesTaxes.Services;

public class CartService : ICartService
{
    private readonly List<CartItemDTO> _cartItems;

    public CartService()
    {
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

    public IList<CartItemDTO> ListCartItems() 
    {
        return _cartItems;
    }
}
