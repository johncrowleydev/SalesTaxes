using SalesTaxes.Common.DTOs;

namespace SalesTaxes.Contracts.Services;

public interface ICartService
{
    CartItemDTO AddCartItem(CartItemDTO item);
    void ClearCartItems();
    IList<CartItemDTO> ListCartItems();
}
