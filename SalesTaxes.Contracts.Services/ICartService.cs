using SalesTaxes.Common.DTOs;

namespace SalesTaxes.Contracts.Services;

public interface ICartService
{
    CartItemDTO AddCartItem(CartItemDTO item);
    void ClearCartItems();
    int GetCartItemsCount();
    IList<CartItemDTO> ListCartItems();
    void RemoveCartItem(Guid id);
    CartItemDTO UpdateCartItemQuantity(Guid id, int quantity);
}
