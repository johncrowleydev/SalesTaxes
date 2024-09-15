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
